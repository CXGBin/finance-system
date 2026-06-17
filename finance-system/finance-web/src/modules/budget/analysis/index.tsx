import React, { useState, useEffect } from 'react';
import { Card, Select, Button, Row, Col, Statistic, Spin, Alert, Empty } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import { budgetApi } from '@/api/budget';
import dayjs from 'dayjs';

/** 预算分析 */
const BudgetAnalysis: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [overview, setOverview] = useState<Record<string, any>>({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadOverview = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await budgetApi.analysisOverview(year);
      setOverview(res.data ?? {});
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载预算分析数据失败');
    } finally { setLoading(false); }
  };

  useEffect(() => { loadOverview(); }, [year]);

  return (
    <Card title="预算分析" extra={
      <Space>
        <Select value={year} onChange={setYear} style={{ width: 120 }}
          options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <Button icon={<ReloadOutlined />} onClick={loadOverview}>刷新</Button>
      </Space>
    }>
      <Row gutter={16} style={{ marginBottom: 24 }}>
        <Col span={6}><Statistic title="年度预算总额" value={overview.totalBudget ?? 0} precision={2} prefix="¥" loading={loading} /></Col>
        <Col span={6}><Statistic title="已执行金额" value={overview.executedAmount ?? 0} precision={2} prefix="¥" loading={loading} /></Col>
        <Col span={6}><Statistic title="执行率" value={overview.executionRate ?? 0} precision={1} suffix="%" loading={loading} /></Col>
        <Col span={6}><Statistic title="超预警科目" value={overview.overBudgetCount ?? 0} loading={loading} valueStyle={{ color: (overview.overBudgetCount ?? 0) > 0 ? '#ff4d4f' : '#3f8600' }} /></Col>
      </Row>
      {error ? (
        <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadOverview}>重试</Button>} />
      ) : null}
      <Card><div style={{ textAlign: 'center', padding: 60, color: '#999' }}>预算分析图表开发中</div></Card>
    </Card>
  );
};

export default BudgetAnalysis;
