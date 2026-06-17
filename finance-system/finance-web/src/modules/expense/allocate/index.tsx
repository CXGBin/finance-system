import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { expenseApi } from '@/api/expense';

/** 费用分摊 */
const ExpenseAllocate: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '分摊单号', dataIndex: 'allocateNo', search: true, sorter: true },
    { title: '费用说明', dataIndex: 'description', search: true },
    { title: '分摊总额', dataIndex: 'totalAmount', align: 'right', sorter: true },
    { title: '分摊部门', dataIndex: 'deptName', search: true },
    { title: '分摊金额', dataIndex: 'allocateAmount', align: 'right', sorter: true },
  ];
  const request = createProTableRequest((params) => expenseApi.allocateList(params));
  return <Card title="费用分摊"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto' }} rowKey="id" /></Card>;
};

export default ExpenseAllocate;
