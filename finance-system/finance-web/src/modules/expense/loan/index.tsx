import React, { useState, useRef } from 'react';
import { Modal, Form, InputNumber, Input, Space, message, Tag, Popconfirm } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { loanApi } from '@/api/expense';

export default function ExpenseLoanPage() {
  const [modalVisible, setModalVisible] = useState(false);
  const [form] = Form.useForm();
  const actionRef = useRef<ActionType>();

  const handleCreate = async () => {
    try {
      const values = await form.validateFields();
      await loanApi.create(values);
      message.success('借款申请已提交');
      setModalVisible(false);
      form.resetFields();
      actionRef.current?.reload();
    } catch {}
  };

  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '借款编号', dataIndex: 'loanNo', search: true, sorter: true, width: 120 },
    { title: '借款金额', dataIndex: 'loanAmount', align: 'right', sorter: true, width: 120 },
    { title: '已核销金额', dataIndex: 'settledAmount', align: 'right', sorter: true, width: 120, search: false },
    { title: '借款事由', dataIndex: 'reason', search: true, ellipsis: true },
    { title: '预计还款日', dataIndex: 'expectedReturnDate', sorter: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '待审批' }, 1: { text: '已借出' }, 2: { text: '已核销' }, 3: { text: '已退回' } }, search: true },
    { title: '创建时间', dataIndex: 'createdTime', sorter: true },
    {
      title: '操作', key: 'action', search: false, width: 180, fixed: 'right' as const,
      render: (_, record) => {
        const r = record as any;
        return (
          <Space>
            {r.status === 0 && (
              <>
                <a onClick={() => { loanApi.approve(r.id); message.success('已审批'); setTimeout(() => actionRef.current?.reload(), 500); }}>审批</a>
                <Popconfirm title="确认退回？" onConfirm={() => { loanApi.reject(r.id); message.success('已退回'); setTimeout(() => actionRef.current?.reload(), 500); }}>
                  <a style={{ color: '#ff4d4f' }}>退回</a>
                </Popconfirm>
              </>
            )}
          </Space>
        );
      },
    },
  ];
  const request = createProTableRequest((params) => loanApi.list(params));

  return (
    <div>
      <ProTable
        actionRef={actionRef}
        columns={columns}
        request={request}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        rowKey="id"
        scroll={{ x: 1200 }}
        toolBarRender={() => [<a key="add" onClick={() => setModalVisible(true)}>申请借款</a>]}
      />
      <Modal title="申请借款" open={modalVisible} onOk={handleCreate} onCancel={() => setModalVisible(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="loanAmount" label="借款金额" rules={[{ required: true }]}><InputNumber min={0.01} precision={2} style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="reason" label="借款事由"><Input.TextArea rows={3} /></Form.Item>
          <Form.Item name="expectedReturnDate" label="预计还款日期"><Input type="date" /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
