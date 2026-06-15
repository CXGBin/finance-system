import React from 'react';
import { Card } from 'antd';
import ProTable from '@/components/ProTable';
import { expenseApi } from '@/api/expense';
import type { ExpenseAllocate } from '@/types/expense.d';

/** 费用分摊 */
const ExpenseAllocate: React.FC = () => {
  const columns = [
    { title: '分摊单号', dataIndex: 'allocateNo', key: 'allocateNo', search: true },
    { title: '费用说明', dataIndex: 'description', key: 'description' },
    { title: '分摊总额', dataIndex: 'totalAmount', key: 'totalAmount', align: 'right' },
    { title: '分摊部门', dataIndex: 'deptName', key: 'deptName' },
    { title: '分摊金额', dataIndex: 'allocateAmount', key: 'allocateAmount', align: 'right' },
  ];
  return <Card title="费用分摊"><ProTable columns={columns} fetchData={(params) => expenseApi.allocateList(params as any)} /></Card>;
};

export default ExpenseAllocate;
