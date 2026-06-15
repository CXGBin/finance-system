import React, { useState } from 'react';
import { Card, Table, Select, Space, Button } from 'antd';
import { reportApi } from '@/api/report';
import type { BalanceSheetRow } from '@/types/report.d';
import dayjs from 'dayjs';

/** 资产负债表 */
const BalanceSheet: React.FC = () => {
  const [data, setData] = useState<BalanceSheetRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ year: dayjs().year(), month: dayjs().month() + 1 });

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await reportApi.balanceSheet(params as any);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const columns = [
    { title: '项目', dataIndex: 'itemName', key: 'itemName', width: 200 },
    { title: '行次', dataIndex: 'lineNo', key: 'lineNo', width: 60, align: 'center' },
    { title: '期末余额', dataIndex: 'endBalance', key: 'endBalance', align: 'right' },
    { title: '年初余额', dataIndex: 'beginBalance', key: 'beginBalance', align: 'right' },
  ];

  return (
    <Card title="资产负债表" extra={<Button type="primary" onClick={loadData} loading={loading}>查询</Button>}>
      <Space style={{ marginBottom: 16 }}>
        <Select value={params.year} onChange={(v) => setParams({ ...params, year: v })} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <span>年</span>
        <Select value={params.month} onChange={(v) => setParams({ ...params, month: v })} style={{ width: 80 }} options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} />
        <span>月</span>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="lineNo" loading={loading} pagination={false} />
    </Card>
  );
};

export default BalanceSheet;
