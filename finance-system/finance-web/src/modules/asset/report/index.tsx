import React, { useState } from 'react';
import { Card, Select, Button, Row, Col, Statistic } from 'antd';
import { assetApi } from '@/api/asset';
import dayjs from 'dayjs';

/** 资产报表 */
const AssetReport: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await assetApi.reportData({ year }); setData(res.data as { totalCount: number; totalOriginal: number; totalDepreciation: number; totalNet: number } | null); } finally { setLoading(false); };
  };

  return (
    <Card title="资产报表">
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col><Select value={year} onChange={setYear} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} /></Col>
        <Col><Button type="primary" onClick={loadData} loading={loading}>生成</Button></Col>
      </Row>
      {data && (
        <Row gutter={16}>
          <Col span={6}><Statistic title="资产总数" value={data.totalCount} /></Col>
          <Col span={6}><Statistic title="资产原值合计" value={data.totalOriginal} precision={2} /></Col>
          <Col span={6}><Statistic title="累计折旧合计" value={data.totalDepreciation} precision={2} /></Col>
          <Col span={6}><Statistic title="净值合计" value={data.totalNet} precision={2} /></Col>
        </Row>
      )}
    </Card>
  );
};

export default AssetReport;
