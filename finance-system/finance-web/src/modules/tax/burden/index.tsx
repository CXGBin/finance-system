import React, { useState } from 'react';
import { Card, Select, Statistic, Row, Col } from 'antd';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';

/** 税负分析 */
const TaxBurden: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());

  return (
    <Card title="税负分析">
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <Select value={year} onChange={setYear} style={{ width: 120 }}
            options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={8}><Card><Statistic title="增值税" value={0} suffix="元" /></Card></Col>
        <Col span={8}><Card><Statistic title="企业所得税" value={0} suffix="元" /></Card></Col>
        <Col span={8}><Card><Statistic title="综合税负率" value={0} suffix="%" /></Card></Col>
      </Row>
      <Card style={{ marginTop: 16 }}><div style={{ textAlign: 'center', padding: 60, color: '#999' }}>税负分析图表开发中</div></Card>
    </Card>
  );
};

export default TaxBurden;
