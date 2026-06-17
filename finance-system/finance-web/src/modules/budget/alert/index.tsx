import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { budgetApi } from '@/api/budget';

/** 预算预警页面 */
const BudgetAlert: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '科目名称', dataIndex: 'subjectName', search: true, sorter: true },
    { title: '预算金额', dataIndex: 'budgetAmount', align: 'right', sorter: true },
    { title: '已用金额', dataIndex: 'usedAmount', align: 'right', sorter: true },
    { title: '预警阈值(%)', dataIndex: 'alertThreshold', align: 'right', sorter: true },
    { title: '预警方式', dataIndex: 'alertMethod', valueType: 'select', valueEnum: { 1: { text: '站内消息' }, 2: { text: '邮件' } }, search: true },
    { title: '状态', dataIndex: 'isEnabled', valueType: 'select', valueEnum: { 0: { text: '禁用' }, 1: { text: '启用' } }, search: true },
  ];
  const request = createProTableRequest((params) => budgetApi.alertCheck({ ...params } as any));
  return <Card title="预算预警"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto', defaultCollapsed: true }} rowKey="id" /></Card>;
};

export default BudgetAlert;
