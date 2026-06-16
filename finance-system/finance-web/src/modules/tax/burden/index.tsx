import React, { useState, useEffect } from 'react';
import { Card, Select, Statistic, Row, Col, Spin, message } from 'antd';
import { taxApi } from '@/api/tax';

/** 税负分析 */
export default function TaxBurdenPage() {
  const [year, setYear] = useState(new Date().getFullYear());
  const [quarter, setQuarter] = useState<number | undefined>(undefined);
  const [data, setData] = useState<Record<string, unknown>>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    taxApi.reportBurden(year, quarter)
      .then(res => setData(res.data || {}))
      .catch(() => { message.error('加载税负分析数据失败'); })
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
          <Col span={6}><Card><Statistic title="营业收入" value={Number(data.totalRevenue || 0)} prefix="¥" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="增值税" value={Number(data.vatPaid || 0)} prefix="¥" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="增值税税负率" value={Number(data.vatBurdenRate || 0)} suffix="%" precision={2} /></Card></Col>
          <Col span={6}><Card><Statistic title="综合税负率" value={Number(data.totalBurdenRate || 0)} suffix="%" precision={2} /></Card></Col>
        </Row>
        <Row gutter={16}>
          <Col span={12}><Card><Statistic title="全部已缴税费合计" value={Number(data.totalTaxPaid || 0)} prefix="¥" precision={2} valueStyle={{ color: '#cf1322' }} /></Card></Col>
        </Row>
      </Spin>
    </div>
  );
}
