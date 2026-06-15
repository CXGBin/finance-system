import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, message, Popconfirm, Space } from 'antd';
import type { Post } from '@/types/system.d';
import { postApi } from '@/api/system';
import ProTable from '@/components/ProTable';

/** 岗位管理页面 */
const PostList: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Post | null>(null);
  const [form] = Form.useForm();

  const columns = [
    { title: '岗位编码', dataIndex: 'postCode', key: 'postCode', search: true },
    { title: '岗位名称', dataIndex: 'postName', key: 'postName', search: true },
    { title: '排序', dataIndex: 'sortOrder', key: 'sortOrder' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => (
        <span style={{ color: val === 1 ? '#52c41a' : '#ff4d4f' }}>{val === 1 ? '正常' : '停用'}</span>
      ),
    },
    {
      title: '操作', key: 'action',
      render: (_: unknown, record: Post) => (
        <Space>
          <a className="table-action" onClick={() => handleEdit(record as Post)}>编辑</a>
          <Popconfirm title="确认删除该岗位?" onConfirm={() => handleDelete(record)}>
            <a className="table-action" style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleEdit = (record: Post) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await postApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await postApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
    } catch {}
  };

  /** 删除岗位 */
  const handleDelete = async (record: Post) => {
    await postApi.remove(record.id);
    message.success('删除成功');
  };

  return (
    <div>
      <ProTable
        columns={columns}
        fetchData={(params) => postApi.list(params as any)}
        toolbarActions={<a onClick={handleAdd}>新增岗位</a>}
      />
      <Modal title={editingRecord ? '编辑岗位' : '新增岗位'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="postCode" label="岗位编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="postName" label="岗位名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序" initialValue={0}><InputNumber /></Form.Item>
          <Form.Item name="status" label="状态" initialValue={1}><Input type="number" /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default PostList;
