import React from 'react';
import { Card } from 'antd';
import ProTable from '@/components/ProTable';
import { expenseApi } from '@/api/expense';
import type { PaymentRecord } from '@/types/expense.d';

/** 付款记录 */
const PaymentList: React.FC = () => {
  const columns = [
    { title: '报销单号', dataIndex: 'claimNo', key: 'claimNo', search: true },
    { title: '报销人', dataIndex: 'claimantName', key: 'claimantName' },
    { title: '付款金额', dataIndex: 'paymentAmount', key: 'paymentAmount', align: 'right' },
    { title: '付款日期', dataIndex: 'paymentDate', key: 'paymentDate' },
  ];
  return <Card title="付款记录"><ProTable columns={columns} fetchData={(params) => expenseApi.paymentList(params as any)} /></Card>;
};

export default PaymentList;
