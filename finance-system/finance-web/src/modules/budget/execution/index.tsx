import React, { useState, useRef, useEffect } from 'react';
import { Card, Select, Tag, Space, Row, Col, Statistic } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { budgetApi } from '@/api/budget';
import type { BudgetExecution } from '@/types/budget.d';
import dayjs from 'dayjs';

/** 预算执行跟踪 */
const BudgetExecution: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [years, setYears] = useState<{ id: number; year: number }[]>([]);
  const [yearId, setYearId] = useState<number | undefined>();

  useEffect(() => {
    budgetApi.years().then(res => {
      const list = res.data ?? [];
      setYears(list);
      if (list[0]) setYearId(list[0].id);
    });
  }, []);

  const columns: ProColumns<BudgetExecution>[] = [
    { title: '科目名称', dataIndex: 'subjectName', ellipsis: true, search: false },
    { title: '年度预算', dataIndex: 'annualBudget', align: 'right', search: false, render: (_, r) => <span className="amount-right">¥{(r.annualBudget ?? 0).toFixed(2)}</span> },
    { title: '已执行', dataIndex: 'executedAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">¥{(r.executedAmount ?? 0).toFixed(2)}</span> },
    { title: '执行率(%)', dataIndex: 'executionRate', align: 'right', search: false, render: (_, r) => {
      const rate = r.executionRate ?? ((r.executedAmount ?? 0) / (r.annualBudget ?? 1) * 100);
      const color = rate > 90 ? '#ff4d4f' : rate > 70 ? '#faad14' : '#52c41a';
      return <span style={{ color }}>{rate.toFixed(1)}%</span>;
    }},
    { title: '剩余预算', dataIndex: 'remainingBudget', align: 'right', search: false, render: (_, r) => {
      const remaining = (r.annualBudget ?? 0) - (r.executedAmount ?? 0);
      return <span style={{ color: remaining < 0 ? '#ff4d4f' : undefined }} className="amount-right">¥{remaining.toFixed(2)}</span>;
    }},
  ];

  return (
    <Card title="预算执行跟踪">
      <Space style={{ marginBottom: 16 }}>
        <span>年度：</span>
        <Select value={yearId} onChange={setYearId} style={{ width: 140 }}
          options={years.map(y => ({ label: `${y.year}年`, value: y.id }))} />
      </Space>
      <ProTable<BudgetExecution> actionRef={actionRef} headerTitle="" rowKey="id" columns={columns}
        search={false}
        request={async () => {
          if (!yearId) return { data: [], success: true, total: 0 };
          const res = await budgetApi.execution({ budgetYearId: yearId });
          const list = res.data ?? [];
          return { data: list, success: true, total: list.length };
        }}
        pagination={false}
      />
    </Card>
  );
};

export default BudgetExecution;
