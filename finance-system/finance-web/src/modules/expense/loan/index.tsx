import React, { useState } from 'react';
import { Card, Button, Modal, Form, InputNumber, Input, Space, message, Tag, Popconfirm } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { loanApi } from '@/api/expense';
import ProTable from '@/components/ProTable';
import type { ProTableRef } from '@/components/ProTable';

const statusMap: Record<number, { text: string; color: string }> = {
  0: { text: '待审批', color: 'orange' },
  1: { text: '已借出', color: 'blue' },
  2: { text: '已核销', color: 'green' },
  3: { text: '已退回', color: 'red' },
};

export default function ExpenseLoanPage() {
  const [modalVisible, setModalVisible] = useState(false);
  const [form] = Form.useForm();
  const tableRef = React.useRef<ProTableRef>(null);

  const handleCreate = async () => {
    try {
      const values = await form.validateFields();
      await loanApi.create(values);
      message.success('借款申请已提交');
      setModalVisible(false);
      form.resetFields();
      tableRef.current?.refresh();
    } catch {}
  };

  const columns = [
    { title: '借款编号', dataIndex: 'loanNo', width: 120 },
    { title: '借款金额', dataIndex: 'loanAmount', width: 120, render: (v: number) => `¥${v.toFixed(2)}` },
    { title: '已核销金额', dataIndex: 'settledAmount', width: 120, render: (v: number) => `¥${v.toFixed(2)}` },
    { title: '剩余可核销', width: 120, render: (_: unknown, r: ExpenseLoan) => `¥${(r.loanAmount - r.settledAmount).toFixed(2)}` },
    { title: '借款事由', dataIndex: 'reason', ellipsis: true },
    { title: '预计还款日', dataIndex: 'expectedReturnDate', width: 120 },
    { title: '状态', dataIndex: 'status', width: 100, render: (v: number) => <Tag color={statusMap[v]?.color}>{statusMap[v]?.text}</Tag> },
    { title: '创建时间', dataIndex: 'createdTime', width: 160 },
    {
      title: '操作', width: 180, fixed: 'right' as const,
      render: (_: unknown, r: ExpenseLoan) => (
        <Space>
          {r.status === 0 && (
            <>
              <Button size="small" type="primary" onClick={() => loanApi.approve(r.id).then(() => { message.success('已审批'); tableRef.current?.refresh(); })}>审批</Button>
              <Popconfirm title="确认退回？" onConfirm={() => loanApi.reject(r.id).then(() => { message.success('已退回'); tableRef.current?.refresh(); })}>
                <Button size="small" danger>退回</Button>
              </Popconfirm>
            </>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Card title="借款管理">
      <div style={{ marginBottom: 16 }}>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalVisible(true)}>申请借款</Button>
      </div>
      <ProTable
        ref={tableRef}
        columns={columns}
        fetchData={async (params) => {
          const res = await loanApi.list({ pageIndex: params.pageIndex, pageSize: params.pageSize });
          return { data: { list: res.list || [], total: res.total || 0 } };
        }}
        tableProps={{ scroll: { x: 1200 } }}
      />
      <Modal title="申请借款" open={modalVisible} onOk={handleCreate} onCancel={() => setModalVisible(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="loanAmount" label="借款金额" rules={[{ required: true }]}>
            <InputNumber min={0.01} precision={2} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="reason" label="借款事由">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="expectedReturnDate" label="预计还款日期">
            <Input type="date" />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
