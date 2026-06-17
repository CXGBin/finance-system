import React, { useState, useRef } from 'react';
import { Modal, Form, Input, Select, Switch, message, Popconfirm, Space, Button, Tag, Card } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import { noticeApi, type SysNotice } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 系统公告管理页面 */
const NoticeList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<SysNotice | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<SysNotice>[] = [
    { title: '公告标题', dataIndex: 'title', ellipsis: true },
    {
      title: '类型', dataIndex: 'noticeType', valueType: 'select',
      valueEnum: { 1: { text: '通知', status: 'Processing' }, 2: { text: '公告', status: 'Warning' } },
    },
    {
      title: '状态', dataIndex: 'status', valueType: 'select',
      valueEnum: { 1: { text: '启用', status: 'Success' }, 0: { text: '停用', status: 'Default' } },
    },
    { title: '创建时间', dataIndex: 'createdTime', valueType: 'dateTime', sorter: true, search: false, width: 180 },
    {
      title: '操作', valueType: 'option', width: 180,
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

  const handleAdd = () => { setEditingRecord(null); form.resetFields(); form.setFieldsValue({ noticeType: 1, status: 1 }); setModalOpen(true); };
  const handleEdit = (record: SysNotice) => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); };
  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) { await noticeApi.update(editingRecord.id, values as any); message.success('更新成功'); }
      else { await noticeApi.create(values as any); message.success('新增成功'); }
      setModalOpen(false); actionRef.current?.reload();
    } catch { /* validation */ }
  };
  const handleDelete = async (record: SysNotice) => { await noticeApi.remove(record.id); message.success('删除成功'); actionRef.current?.reload(); };

  return (
    <>
      <ProTable<SysNotice>
        actionRef={actionRef} headerTitle="系统公告" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        request={createProTableRequest((params) => noticeApi.list(params.noticeType as number | undefined, params))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={handleAdd}>发布公告</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal title={editingRecord ? '编辑公告' : '发布公告'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={700}>
        <Form form={form} layout="vertical">
          <Form.Item name="title" label="标题" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="noticeType" label="类型" rules={[{ required: true }]}>
            <Select options={[{ label: '通知', value: 1 }, { label: '公告', value: 2 }]} />
          </Form.Item>
          <Form.Item name="content" label="内容" rules={[{ required: true }]}>
            <Input.TextArea rows={6} />
          </Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked"><Switch checkedChildren="启用" unCheckedChildren="禁用" /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default NoticeList;
