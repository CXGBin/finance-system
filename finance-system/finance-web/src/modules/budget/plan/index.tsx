import React, { useState } from 'react';
import { Card, Table, Button, message } from 'antd';
import { budgetApi } from '@/api/budget';
import ProTable from '@/components/ProTable';
import type { BudgetItem } from '@/types/budget.d';

/** 预算编制页面 */
const BudgetPlan: React.FC = () => {
  const columns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode', search: true },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '年度预算', dataIndex: 'annualBudget', key: 'annualBudget', align: 'right' },
    { title: '已执行', dataIndex: 'executedAmount', key: 'executedAmount', align: 'right' },
    { title: '剩余预算', dataIndex: 'remainingBudget', key: 'remainingBudget', align: 'right' },
  ];

  return (
    <Card title="预算编制">
      <ProTable columns={columns} fetchData={(params) => budgetApi.planList(params as any)} />
    </Card>
  );
};

export default BudgetPlan;
