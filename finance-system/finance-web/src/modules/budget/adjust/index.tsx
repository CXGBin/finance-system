import React, { useRef } from 'react';
import { Card, Modal, Form, InputNumber, Select, Input, message, Space, Button, Tag, Popconfirm } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import { budgetApi } from '@/api/budget';
import type { BudgetAdjust, BudgetItem } from '@/types/budget.d';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 预算调整页面 */
const BudgetAdjust: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = React.useState(false);
  const [form] = Form.useForm();
  const [subjects, setSubjects] = React.useState<BudgetItem[]>([]);

  React.useEffect(() => {
    budgetApi.years().then(res => {
      const yearId = res.data?.[0]?.id;
      if (yearId) budgetApi.subjectList({ yearId, pageIndex: 1, pageSize: 999 }).then(r => setSubjects(r.data?.list ?? []));
    });
  }, []);

  const columns: ProColumns<BudgetAdjust>[] = [
    { title: '科目名称', dataIndex: 'subjectName', ellipsis: true },
    { title: '调整类型', dataIndex: 'adjustType', valueType: 'select', valueEnum: { 1: { text: '增加', status: 'Success' }, 2: { text: '减少', status: 'Error' } } },
    { title: '调整前预算', dataIndex: 'beforeAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.beforeAmount ?? 0).toFixed(2)}</span> },
    { title: '调整后预算', dataIndex: 'afterAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.afterAmount ?? 0).toFixed(2)}</span> },
    { title: '调整原因', dataIndex: 'reason', search: false, ellipsis: true },
    { title: '操作人', dataIndex: 'operatorName', search: false },
    { title: '调整时间', dataIndex: 'createdTime', valueType: 'dateTime', sorter: true, search: false, width: 180 },
    {
      title: '操作', valueType: 'option', width: 120, search: false,
      render: (_, record) => (
        <Space>
          <Popconfirm title="确认审批通过?" onConfirm={() => budgetApi.adjustApprove(record.id, 1).then(() => actionRef.current?.reload())}>
            <a>通过</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Card title="预算调整">
        <ProTable<BudgetAdjust>
          actionRef={actionRef} headerTitle="" rowKey="id" columns={columns}
          search={{ labelWidth: 'auto' }}
          request={createProTableRequest((params) => budgetApi.adjustList(params as any))}
          toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { form.resetFields(); setModalOpen(true); }}>发起调整</Button>]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
      <Modal title="发起预算调整" open={modalOpen} onOk={async () => {
        try {
          const values = await form.validateFields();
          await budgetApi.adjustAdd(values as any);
          message.success('提交成功'); setModalOpen(false); actionRef.current?.reload();
        } catch { /* validation */ }
      }} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="budgetSubjectId" label="预算科目" rules={[{ required: true }]}>
            <Select options={subjects.map(s => ({ label: s.subjectName, value: s.id }))} showSearch optionFilterProp="label" />
          </Form.Item>
          <Form.Item name="adjustType" label="调整类型" rules={[{ required: true }]}>
            <Select options={[{ label: '增加', value: 1 }, { label: '减少', value: 2 }]} />
          </Form.Item>
          <Form.Item name="adjustAmount" label="调整金额" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} precision={2} /></Form.Item>
          <Form.Item name="reason" label="原因"><Input.TextArea rows={3} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default BudgetAdjust;
