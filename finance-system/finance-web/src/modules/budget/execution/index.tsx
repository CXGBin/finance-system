import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Space, Button, message } from 'antd';
import { budgetApi } from '@/api/budget';
import type { BudgetExecution } from '@/types/budget.d';
import dayjs from 'dayjs';

/** 预算执行跟踪 */
const BudgetExecution: React.FC = () => {
  const [data, setData] = useState<BudgetExecution[]>([]);
  const [loading, setLoading] = useState(false);
  const [years, setYears] = useState<{ id: number; year: number; status: number }[]>([]);
  const [selectedYearId, setSelectedYearId] = useState<number | undefined>();

  useEffect(() => {
    budgetApi.years().then((res) => {
      const list = res.data || [];
      setYears(list);
      if (list.length > 0) setSelectedYearId(list[0].id);
    }).catch(() => {});
  }, []);

  const loadData = async () => {
    if (!selectedYearId) return;
    setLoading(true);
    try {
      const res = await budgetApi.execution({ budgetYearId: selectedYearId });
      setData(res.data || []);
    } finally { setLoading(false); }
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
        <Select
          value={selectedYearId}
          onChange={setSelectedYearId}
          style={{ width: 120 }}
          placeholder="选择预算年度"
          options={years.map((y) => ({ label: `${y.year}年`, value: y.id }))}
        />
        <Button type="primary" onClick={loadData} loading={loading}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="budgetSubjectId" loading={loading} pagination={false} />
    </Card>
  );
};

export default BudgetExecution;
