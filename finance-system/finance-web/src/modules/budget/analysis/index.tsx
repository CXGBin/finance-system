import React, { useState } from 'react';
import { Card, Select, Button, Row, Col, Statistic } from 'antd';
import { budgetApi } from '@/api/budget';
import dayjs from 'dayjs';

/** 预算分析页面 */
const BudgetAnalysis: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<{ totalBudget: number; totalExecuted: number; totalRate: number; overBudgetCount: number } | null>(null);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await budgetApi.analysis({ year }); setData(res.data || null); } finally { setLoading(false); }
  };

  return (
    <Card title="预算分析">
      <Select value={year} onChange={setYear} style={{ width: 100, marginBottom: 16 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
      <Button type="primary" onClick={loadData} loading={loading}>分析</Button>
      {data && (
        <Row gutter={16} style={{ marginTop: 16 }}>
          <Col span={6}><Statistic title="年度总预算" value={data.totalBudget} precision={2} /></Col>
          <Col span={6}><Statistic title="已执行金额" value={data.totalExecuted} precision={2} /></Col>
          <Col span={6}><Statistic title="执行率" value={data.totalRate} suffix="%" precision={1} /></Col>
          <Col span={6}><Statistic title="超预算科目数" value={data.overBudgetCount} valueStyle={{ color: '#ff4d4f' }} /></Col>
        </Row>
      )}
    </Card>
  );
};

export default BudgetAnalysis;
