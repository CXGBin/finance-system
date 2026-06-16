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
    { title: '预警阈值(%)', dataIndex: 'alertThreshold', key: 'alertThreshold', align: 'right' },
    { title: '预警方式', dataIndex: 'alertMethod', key: 'alertMethod', render: (val: number) => ({ 1: '站内消息', 2: '邮件' }[val] || '-') },
    { title: '状态', dataIndex: 'isEnabled', key: 'isEnabled', render: (val: number) => val ? <Tag color="green">启用</Tag> : <Tag color="default">禁用</Tag> },
  ];
  return <Card title="预算预警"><ProTable columns={columns} fetchData={(params) => budgetApi.alertList(params as any)} /></Card>;
};

export default BudgetAlert;
