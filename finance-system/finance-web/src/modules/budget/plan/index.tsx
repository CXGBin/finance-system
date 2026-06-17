import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { budgetApi } from '@/api/budget';

/** 预算编制页面 */
const BudgetPlan: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '科目编码', dataIndex: 'subjectCode', search: true, sorter: true },
    { title: '科目名称', dataIndex: 'subjectName', search: true },
    { title: '年度预算', dataIndex: 'annualBudget', align: 'right', sorter: true },
    { title: '已执行', dataIndex: 'executedAmount', align: 'right', sorter: true },
    { title: '剩余预算', dataIndex: 'remainingBudget', align: 'right', sorter: true },
  ];
  const request = createProTableRequest((params) => budgetApi.planList(params));
  return <Card title="预算编制"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto' }} rowKey="id" /></Card>;
};

export default BudgetPlan;
