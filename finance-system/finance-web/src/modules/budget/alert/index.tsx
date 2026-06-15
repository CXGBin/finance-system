import React from 'react';
import { Card, Tag } from 'antd';
import ProTable from '@/components/ProTable';
import { budgetApi } from '@/api/budget';
import type { BudgetAlert } from '@/types/budget.d';

/** 预算预警页面 */
const BudgetAlert: React.FC = () => {
  const columns = [
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName', search: true },
    { title: '预算金额', dataIndex: 'budgetAmount', key: 'budgetAmount', align: 'right' },
    { title: '已用金额', dataIndex: 'usedAmount', key: 'usedAmount', align: 'right' },
    { title: '使用率(%)', dataIndex: 'usageRate', key: 'usageRate', align: 'right' },
    {
      title: '预警等级', dataIndex: 'alertLevel', key: 'alertLevel',
      render: (val: number) => val >= 100 ? <Tag color="red">超预算</Tag> : val >= 80 ? <Tag color="orange">预警</Tag> : <Tag color="green">正常</Tag>,
    },
    { title: '预警时间', dataIndex: 'alertTime', key: 'alertTime' },
  ];
  return <Card title="预算预警"><ProTable columns={columns} fetchData={(params) => budgetApi.alertList(params as any)} /></Card>;
};

export default BudgetAlert;
