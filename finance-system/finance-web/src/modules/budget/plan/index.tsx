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
    { title: '年度预算', dataIndex: 'yearBudget', key: 'yearBudget', align: 'right' },
    { title: '已用预算', dataIndex: 'usedBudget', key: 'usedBudget', align: 'right' },
    { title: '剩余预算', dataIndex: 'remainBudget', key: 'remainBudget', align: 'right' },
  ];

  return (
    <Card title="预算编制">
      <ProTable columns={columns} fetchData={(params) => budgetApi.planList(params as any)} />
    </Card>
  );
};

export default BudgetPlan;
