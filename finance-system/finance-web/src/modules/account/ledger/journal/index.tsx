import React, { useState } from 'react';
import { Card, Table, Select, Space, Button } from 'antd';
import { ledgerApi } from '@/api/account';
import type { LedgerRecord } from '@/types/account.d';
import dayjs from 'dayjs';

/** 日记账页面 */
const JournalLedger: React.FC = () => {
  const [data, setData] = useState<LedgerRecord[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ startPeriod: dayjs().startOf('year').format('YYYY-MM'), endPeriod: dayjs().format('YYYY-MM') });

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await ledgerApi.journal(params);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const columns = [
    { title: '日期', dataIndex: 'voucherDate', key: 'voucherDate' },
    { title: '凭证号', dataIndex: 'voucherNo', key: 'voucherNo' },
    { title: '摘要', dataIndex: 'summary', key: 'summary', ellipsis: true },
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '借方金额', dataIndex: 'debitAmount', key: 'debitAmount', align: 'right' },
    { title: '贷方金额', dataIndex: 'creditAmount', key: 'creditAmount', align: 'right' },
  ];

  return (
    <Card title="日记账">
      <Space style={{ marginBottom: 16 }}>
        <Select value={params.startPeriod} onChange={(v) => setParams({ ...params, startPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <span>至</span>
        <Select value={params.endPeriod} onChange={(v) => setParams({ ...params, endPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <Button type="primary" onClick={loadData}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} scroll={{ y: 500 }} />
    </Card>
  );
};

export default JournalLedger;
