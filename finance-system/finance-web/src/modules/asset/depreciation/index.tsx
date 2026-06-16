import React, { useState } from 'react';
import { Card, Table, Select, Button, message, Statistic, Row, Col } from 'antd';
import { assetApi } from '@/api/asset';
import type { DepreciationRecord } from '@/types/asset.d';
import dayjs from 'dayjs';

/** 资产折旧页面 */
const AssetDepreciation: React.FC = () => {
  const [data, setData] = useState<DepreciationRecord[]>([]);
  const [loading, setLoading] = useState(false);
  const [running, setRunning] = useState(false);
  const [year, setYear] = useState(dayjs().year());
  const [month, setMonth] = useState(dayjs().month() + 1);

  const loadData = async () => {
    setLoading(true);
    try { const res = await assetApi.depreciationList({ pageIndex: 1, pageSize: 999 }); const d = res.data; setData(Array.isArray(d) ? d : ((d as { list?: typeof data })?.list || [])); } finally { setLoading(false); }
  };

  const handleRun = async () => {
    setRunning(true);
    try {
      await assetApi.depreciationRun(`${year}-${String(month).padStart(2, '0')}`);
      message.success('折旧计算完成');
      loadData();
    } finally { setRunning(false); }
  };

  const columns = [
    { title: '资产ID', dataIndex: 'assetCardId', key: 'assetCardId' },
    { title: '月份', dataIndex: 'month', key: 'month' },
    { title: '折旧额', dataIndex: 'depreciationAmount', key: 'depreciationAmount', align: 'right' },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', key: 'accumulatedDepreciation', align: 'right' },
    { title: '净值', dataIndex: 'netValue', key: 'netValue', align: 'right' },
  ];

  return (
    <Card title="资产折旧" extra={<Button type="primary" onClick={handleRun} loading={running}>执行本月折旧</Button>}>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}><Select value={year} onChange={setYear} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} /><span style={{ marginLeft: 8 }}>年</span></Col>
        <Col span={6}><Select value={month} onChange={setMonth} style={{ width: 80 }} options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} /><span style={{ marginLeft: 8 }}>月</span></Col>
        <Col span={6}><Button onClick={loadData} loading={loading}>查询</Button></Col>
      </Row>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} />
    </Card>
  );
};

export default AssetDepreciation;
