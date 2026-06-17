import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { expenseApi } from '@/api/expense';

/** 付款记录 */
const PaymentList: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '报销单号', dataIndex: 'claimNo', search: true, sorter: true },
    { title: '报销人', dataIndex: 'claimantName', search: true },
    { title: '付款金额', dataIndex: 'paymentAmount', align: 'right', sorter: true },
    { title: '付款日期', dataIndex: 'paymentDate', sorter: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '待付款' }, 1: { text: '已付款' } }, search: true },
  ];
  const request = createProTableRequest((params) => expenseApi.paymentList(params));
  return <Card title="付款记录"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto' }} rowKey="id" /></Card>;
};

export default PaymentList;
