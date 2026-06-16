import React, { useState, useEffect } from 'react';
import { Card, Table, Modal, Form, Input, Select, Space, Button, message, Popconfirm, Tag } from 'antd';
import { noticeApi, type SysNotice } from '@/api/system';

/** 系统公告管理页面 */
const NoticeList: React.FC = () => {
  const [data, setData] = useState<SysNotice[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<SysNotice | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await noticeApi.list();
      setData(res.data || []);
    } catch {
      message.error('加载公告列表失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadData(); }, []);

  /** 打开新增/编辑弹窗 */
  const handleOpenModal = (record?: SysNotice) => {
    setEditingRecord(record || null);
    form.setFieldsValue(record ? { title: record.title, content: record.content, noticeType: record.noticeType, status: record.status } : { noticeType: 1, status: 1 });
    setModalOpen(true);
  };

  /** 提交表单 */
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await noticeApi.update(editingRecord.id, values);
        message.success('更新成功');
      } else {
        await noticeApi.create(values);
        message.success('创建成功');
      }
      setModalOpen(false);
      form.resetFields();
      loadData();
    } catch (err) {
      if (err instanceof Error) message.error(err.message);
    }
  };

  /** 删除公告 */
  const handleDelete = async (id: number) => {
    try {
      await noticeApi.remove(id);
      message.success('删除成功');
      loadData();
    } catch {
      message.error('删除失败');
    }
  };

  const typeMap: Record<number, { color: string; text: string }> = {
    1: { color: 'blue', text: '通知' },
    2: { color: 'orange', text: '公告' },
    3: { color: 'red', text: '预警' },
  };
  const statusMap: Record<number, { color: string; text: string }> = {
    0: { color: 'default', text: '已关闭' },
    1: { color: 'success', text: '已发布' },
  };

  const columns = [
    { title: '标题', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: '类型', dataIndex: 'noticeType', key: 'noticeType', width: 80, render: (v: number) => <Tag color={typeMap[v]?.color}>{typeMap[v]?.text || '未知'}</Tag> },
    { title: '状态', dataIndex: 'status', key: 'status', width: 80, render: (v: number) => <Tag color={statusMap[v]?.color}>{statusMap[v]?.text || '未知'}</Tag> },
    { title: '创建人', dataIndex: 'createdBy', key: 'createdBy', width: 100 },
    { title: '创建时间', dataIndex: 'createdTime', key: 'createdTime', width: 170 },
    {
      title: '操作', key: 'action', width: 150,
      render: (_: unknown, record: SysNotice) => (
        <Space>
          <a onClick={() => handleOpenModal(record)}>编辑</a>
          <Popconfirm title="确认删除该公告？" onConfirm={() => handleDelete(record.id)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <Card title="系统公告" extra={<Button type="primary" onClick={() => handleOpenModal()}>新增公告</Button>}>
      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        loading={loading}
        locale={{ emptyText: '暂无公告数据' }}
        pagination={{ showSizeChanger: true, showTotal: (t) => `共 ${t} 条` }}
      />
      <Modal
        title={editingRecord ? '编辑公告' : '新增公告'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); form.resetFields(); }}
        destroyOnClose
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item label="公告标题" name="title" rules={[{ required: true, message: '请输入公告标题' }, { max: 100, message: '标题不超过100字' }]}>
            <Input placeholder="请输入公告标题" />
          </Form.Item>
          <Form.Item label="公告类型" name="noticeType" rules={[{ required: true, message: '请选择公告类型' }]}>
            <Select options={[{ label: '通知', value: 1 }, { label: '公告', value: 2 }, { label: '预警', value: 3 }]} />
          </Form.Item>
          <Form.Item label="公告内容" name="content" rules={[{ required: true, message: '请输入公告内容' }]}>
            <Input.TextArea rows={4} placeholder="请输入公告内容" />
          </Form.Item>
          <Form.Item label="状态" name="status" rules={[{ required: true, message: '请选择状态' }]}>
            <Select options={[{ label: '已发布', value: 1 }, { label: '已关闭', value: 0 }]} />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
};

export default NoticeList;
