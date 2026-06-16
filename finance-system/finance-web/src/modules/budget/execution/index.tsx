import React, { useState } from 'react';
import { Card, Table, Select, Space, Button } from 'antd';
import { budgetApi } from '@/api/budget';
import type { BudgetExecution } from '@/types/budget.d';
import dayjs from 'dayjs';

/** 预算执行跟踪 */
const BudgetExecution: React.FC = () => {
  const [data, setData] = useState<BudgetExecution[]>([]);
  const [loading, setLoading] = useState(false);
  const [year, setYear] = useState(dayjs().year());

  const loadData = async () => {
    setLoading(true);
    try { const res = await budgetApi.execution({ year }); setData(res.data || []); } finally { setLoading(false); }
  };

  const columns = [
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '部门', dataIndex: 'deptName', key: 'deptName' },
    { title: '年度预算', dataIndex: 'annualBudget', key: 'annualBudget', align: 'right' },
    { title: '已执行', dataIndex: 'executedAmount', key: 'executedAmount', align: 'right' },
    { title: '执行率(%)', dataIndex: 'executionRate', key: 'executionRate', align: 'right' },
    { title: '剩余', dataIndex: 'remainingBudget', key: 'remainingBudget', align: 'right' },
  ];

  return (
    <Card title="预算执行">
      <Space style={{ marginBottom: 16 }}>
        <Select value={year} onChange={setYear} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <span>年</span>
        <Button type="primary" onClick={loadData} loading={loading}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="budgetSubjectId" loading={loading} pagination={false} />
    </Card>
  );
};

export default BudgetExecution;
