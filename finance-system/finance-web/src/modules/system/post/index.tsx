import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, Switch, message, Popconfirm, Space, Button } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import type { Post } from '@/types/system.d';
import { postApi } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 岗位管理页面 */
const PostList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Post | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<Post>[] = [
    { title: '岗位编码', dataIndex: 'postCode', ellipsis: true },
    { title: '岗位名称', dataIndex: 'postName', ellipsis: true },
    {
      title: '状态', dataIndex: 'status', valueType: 'select',
      valueEnum: { 1: { text: '启用', status: 'Success' }, 0: { text: '停用', status: 'Default' } },
    },
    { title: '排序', dataIndex: 'sortOrder', sorter: true, search: false, width: 80, align: 'right' },
    { title: '创建时间', dataIndex: 'createdTime', valueType: 'dateTime', sorter: true, search: false, width: 180 },
    {
      title: '操作', valueType: 'option', width: 140,
      render: (_, record) => (
        <Space>
          <a onClick={() => handleEdit(record)}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => handleDelete(record)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => { setEditingRecord(null); form.resetFields(); form.setFieldsValue({ status: 1, sortOrder: 0 }); setModalOpen(true); };
  const handleEdit = (record: Post) => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); };
  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) { await postApi.update({ ...editingRecord, ...values }); message.success('更新成功'); }
      else { await postApi.add(values as any); message.success('新增成功'); }
      setModalOpen(false); actionRef.current?.reload();
    } catch { /* validation */ }
  };
  const handleDelete = async (record: Post) => { await postApi.remove(record.id); message.success('删除成功'); actionRef.current?.reload(); };

  return (
    <>
      <ProTable<Post>
        actionRef={actionRef} headerTitle="岗位管理" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        request={createProTableRequest((params) => postApi.list(params as any))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={handleAdd}>新增岗位</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal title={editingRecord ? '编辑岗位' : '新增岗位'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="postName" label="岗位名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="postCode" label="岗位编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked"><Switch checkedChildren="启用" unCheckedChildren="禁用" /></Form.Item>
          <Form.Item name="remark" label="备注"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default PostList;
