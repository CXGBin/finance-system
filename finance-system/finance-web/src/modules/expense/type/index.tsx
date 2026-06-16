import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, message, Popconfirm, Space, Button } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import { expenseApi } from '@/api/expense';
import type { ExpenseType } from '@/types/expense.d';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 费用类型管理 */
const ExpenseTypeList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<ExpenseType | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<ExpenseType>[] = [
    { title: '类型编码', dataIndex: 'typeCode', ellipsis: true },
    { title: '类型名称', dataIndex: 'typeName', ellipsis: true },
    { title: '单次限额', dataIndex: 'singleLimit', sorter: true, search: false, align: 'right', render: (_, r) => r.singleLimit ? `¥${r.singleLimit.toFixed(2)}` : '-' },
    { title: '月度限额', dataIndex: 'monthlyLimit', sorter: true, search: false, align: 'right', render: (_, r) => r.monthlyLimit ? `¥${r.monthlyLimit.toFixed(2)}` : '-' },
    { title: '排序', dataIndex: 'sortOrder', sorter: true, search: false, width: 80, align: 'right' },
    {
      title: '操作', valueType: 'option', width: 140,
      render: (_, record) => (
        <Space>
          <a onClick={() => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => { expenseApi.removeType(record.id); message.success('删除成功'); actionRef.current?.reload(); }}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <ProTable<ExpenseType> actionRef={actionRef} headerTitle="费用类型" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => expenseApi.typeList(params as any))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { setEditingRecord(null); form.resetFields(); form.setFieldsValue({ sortOrder: 0 }); setModalOpen(true); }}>新增类型</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal title={editingRecord ? '编辑费用类型' : '新增费用类型'} open={modalOpen} onOk={async () => {
        try {
          const values = await form.validateFields();
          if (editingRecord) { await expenseApi.updateType({ ...editingRecord, ...values }); message.success('更新成功'); }
          else { await expenseApi.addType(values as any); message.success('新增成功'); }
          setModalOpen(false); actionRef.current?.reload();
        } catch { /* validation */ }
      }} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="typeCode" label="类型编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="typeName" label="类型名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="singleLimit" label="单次限额"><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item>
          <Form.Item name="monthlyLimit" label="月度限额"><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default ExpenseTypeList;
