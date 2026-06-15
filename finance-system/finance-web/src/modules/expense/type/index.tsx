import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, Button, message, Popconfirm, Space } from 'antd';
import { Table } from 'antd';
import { expenseApi } from '@/api/expense';
import type { ExpenseType } from '@/types/expense.d';

/** 费用类型管理 */
const ExpenseType: React.FC = () => {
  const [data, setData] = useState<ExpenseType[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<ExpenseType | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => { const res = await expenseApi.typeList(); setData(res.data || []); };

  React.useEffect(() => { loadData(); }, []);

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) { await expenseApi.typeUpdate({ ...editingRecord, ...values }); message.success('更新成功'); }
      else { await expenseApi.typeAdd(values as any); message.success('新增成功'); }
      setModalOpen(false); loadData();
    } catch {}
  };

  const columns = [
    { title: '编码', dataIndex: 'typeCode', key: 'typeCode' },
    { title: '名称', dataIndex: 'typeName', key: 'typeName' },
    { title: '月度限额', dataIndex: 'monthlyLimit', key: 'monthlyLimit', align: 'right' },
    { title: '操作', key: 'action', render: (_: unknown, record: ExpenseType) => (
      <Space>
        <a onClick={() => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); }}>编辑</a>
        <Popconfirm title="确认删除?" onConfirm={() => expenseApi.typeRemove(record.id).then(() => { message.success('删除成功'); loadData(); })}><a style={{ color: '#ff4d4f' }}>删除</a></Popconfirm>
      </Space>
    )},
  ];

  return (
    <div>
      <Button type="primary" onClick={() => { setEditingRecord(null); form.resetFields(); setModalOpen(true); }} style={{ marginBottom: 16 }}>新增费用类型</Button>
      <Table columns={columns} dataSource={data} rowKey="id" />
      <Modal title={editingRecord ? '编辑' : '新增'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="typeCode" label="编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="typeName" label="名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="monthlyLimit" label="月度限额"><InputNumber style={{ width: '100%' }} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ExpenseType;
