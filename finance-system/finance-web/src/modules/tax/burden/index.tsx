import React, { useState, useEffect } from 'react';
import { Card, Select, Statistic, Row, Col, Spin } from 'antd';

export default function TaxBurdenPage() {
  const [year, setYear] = useState(new Date().getFullYear());
  const [quarter, setQuarter] = useState<number | undefined>(undefined);
  const [data, setData] = useState<any>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    const params = new URLSearchParams({ year: String(year) });
    if (quarter) params.set('quarter', String(quarter));
    fetch('/api/tax/report/burden?' + params, {
      headers: { Authorization: 'Bearer ' + localStorage.getItem('token') },
    })
      .then(r => r.json())
      .then(r => setData(r.data || {}))
      .finally(() => setLoading(false));
  }, [year, quarter]);

  return (
    <div>
      <Card style={{ marginBottom: 16 }}>
        <Row gutter={16}>
          <Col span={6}>
            <Select value={year} onChange={setYear} style={{ width: '100%' }}
              options={Array.from({ length: 5 }, (_, i) => ({ value: new Date().getFullYear() - i, label: `${new Date().getFullYear() - i}年` }))} />
          </Col>
          <Col span={6}>
            <Select value={quarter} onChange={setQuarter} allowClear placeholder="全年" style={{ width: '100%' }}
              options={[{ value: 1, label: '第一季度' }, { value: 2, label: '第二季度' }, { value: 3, label: '第三季度' }, { value: 4, label: '第四季度' }]} />
          </Col>
        </Row>
      </Card>
      <Spin spinning={loading}>
        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col span={6}><Card><Statistic title="营业收入" value={data.totalRevenue || 0} prefix="¥" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="增值税" value={data.vatPaid || 0} prefix="¥" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="增值税税负率" value={data.vatBurdenRate || 0} suffix="%" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="综合税负率" value={data.totalBurdenRate || 0} suffix="%" precision={2} /></Card></Col>
        </Row>
        <Row gutter={16}>
          <Col span={12}><Card><Statistic title="全部已缴税费合计" value={data.totalTaxPaid || 0} prefix="¥" precision={2} valueStyle={{ color: '#cf1322' }} /></Card></Col>
        </Row>
      </Spin>
    </div>
  );
}
