import React, { useState } from 'react';
import { Card, Table, Select, Button, message, Row, Col, Statistic } from 'antd';
import { CalculatorOutlined, SearchOutlined } from '@ant-design/icons';
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
    try {
      const res = await assetApi.depreciationCalculate(year, month);
      setData(res.data ?? []);
    } finally { setLoading(false); }
  };

  const handleRun = async () => {
    setRunning(true);
    try {
      await assetApi.depreciationConfirm(year, month);
      message.success(`${year}年${month}月折旧计算完成`);
      loadData();
    } finally { setRunning(false); }
  };

  const depTotal = data.reduce((s, r) => s + (r.depreciationAmount ?? 0), 0);
  const accTotal = data.reduce((s, r) => s + (r.accumulatedDepreciation ?? 0), 0);
  const netTotal = data.reduce((s, r) => s + (r.netValue ?? 0), 0);

  const columns = [
    { title: '资产编码', dataIndex: 'assetCode', ellipsis: true },
    { title: '资产名称', dataIndex: 'assetName', ellipsis: true },
    { title: '折旧额', dataIndex: 'depreciationAmount', align: 'right', render: (v: number) => <span className="amount-right">{(v ?? 0).toFixed(2)}</span> },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', align: 'right', render: (v: number) => <span className="amount-right">{(v ?? 0).toFixed(2)}</span> },
    { title: '净值', dataIndex: 'netValue', align: 'right', render: (v: number) => <span className="amount-right">{(v ?? 0).toFixed(2)}</span> },
  ];

  return (
    <Card title="资产折旧" extra={<Button type="primary" icon={<CalculatorOutlined />} onClick={handleRun} loading={running}>执行折旧</Button>}>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={4}>
          <Select value={year} onChange={setYear} style={{ width: 100 }}
            options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
          <span style={{ marginLeft: 8 }}>年</span>
        </Col>
        <Col span={4}>
          <Select value={month} onChange={setMonth} style={{ width: 80 }}
            options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} />
          <span style={{ marginLeft: 8 }}>月</span>
        </Col>
        <Col span={4}><Button icon={<SearchOutlined />} onClick={loadData} loading={loading}>查询</Button></Col>
      </Row>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={8}><Statistic title="本月折旧合计" value={depTotal} precision={2} /></Col>
        <Col span={8}><Statistic title="累计折旧合计" value={accTotal} precision={2} /></Col>
        <Col span={8}><Statistic title="净值合计" value={netTotal} precision={2} /></Col>
      </Row>
      <Table columns={columns} dataSource={data} rowKey={(r) => `${r.assetCardId}-${r.month}`} loading={loading} pagination={false} />
    </Card>
  );
};

export default AssetDepreciation;
