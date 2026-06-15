import React from 'react';
import { Card } from 'antd';
import ProTable from '@/components/ProTable';
import { budgetApi } from '@/api/budget';
import type { BudgetAdjust } from '@/types/budget.d';

/** 预算调整页面 */
const BudgetAdjust: React.FC = () => {
  const columns = [
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName', search: true },
    { title: '调整前预算', dataIndex: 'beforeAmount', key: 'beforeAmount', align: 'right' },
    { title: '调整后预算', dataIndex: 'afterAmount', key: 'afterAmount', align: 'right' },
    { title: '调整原因', dataIndex: 'reason', key: 'reason', ellipsis: true },
    { title: '操作人', dataIndex: 'operatorName', key: 'operatorName' },
    { title: '调整时间', dataIndex: 'createdTime', key: 'createdTime' },
  ];
  return <Card title="预算调整"><ProTable columns={columns} fetchData={(params) => budgetApi.adjustList(params as any)} /></Card>;
};

export default BudgetAdjust;
