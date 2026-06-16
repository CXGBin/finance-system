import React, { useState } from 'react';
import { Card, Table, Select, Space, Button, Input } from 'antd';
import { reportApi } from '@/api/report';
import type { SubjectBalanceRow } from '@/types/report.d';
import dayjs from 'dayjs';

/** 科目余额表 */
const SubjectBalance: React.FC = () => {
  const [data, setData] = useState<SubjectBalanceRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ year: dayjs().year(), month: dayjs().month() + 1, subjectCode: '', level: 1 });

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.subjectBalance(params as any); setData(res.data || []); } finally { setLoading(false); }
  };

  const columns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '期初借方', dataIndex: 'openingDebit', key: 'openingDebit', align: 'right' },
    { title: '期初贷方', dataIndex: 'openingCredit', key: 'openingCredit', align: 'right' },
    { title: '本期借方', dataIndex: 'currentDebit', key: 'currentDebit', align: 'right' },
    { title: '本期贷方', dataIndex: 'currentCredit', key: 'currentCredit', align: 'right' },
    { title: '期末借方', dataIndex: 'closingDebit', key: 'closingDebit', align: 'right' },
    { title: '期末贷方', dataIndex: 'closingCredit', key: 'closingCredit', align: 'right' },
  ];

  return (
    <Card title="科目余额表" extra={<Button type="primary" onClick={loadData} loading={loading}>查询</Button>}>
      <Space style={{ marginBottom: 16 }}>
        <Input placeholder="科目编码" value={params.subjectCode} onChange={(e) => setParams({ ...params, subjectCode: e.target.value })} allowClear style={{ width: 150 }} />
        <Select value={params.level} onChange={(v) => setParams({ ...params, level: v })} style={{ width: 100 }} options={[{ label: '一级', value: 1 }, { label: '全部', value: 0 }]} />
        <Select value={params.year} onChange={(v) => setParams({ ...params, year: v })} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <span>年</span>
        <Select value={params.month} onChange={(v) => setParams({ ...params, month: v })} style={{ width: 80 }} options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} />
        <span>月</span>
        <Button type="primary" onClick={loadData} loading={loading}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="subjectCode" loading={loading} pagination={false} />
    </Card>
  );
};

export default SubjectBalance;
