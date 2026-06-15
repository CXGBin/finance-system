import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Space, Button } from 'antd';
import { ledgerApi, subjectApi } from '@/api/account';
import type { Subject, LedgerRecord } from '@/types/account.d';
import dayjs from 'dayjs';

/** 明细账页面 */
const DetailLedger: React.FC = () => {
  const [data, setData] = useState<LedgerRecord[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ subjectId: undefined as number | undefined, startPeriod: dayjs().startOf('year').format('YYYY-MM'), endPeriod: dayjs().format('YYYY-MM') });

  useEffect(() => { loadSubjects(); }, []);

  const loadSubjects = async () => {
    const res = await subjectApi.list();
    setSubjects(res.data || []);
  };

  const loadData = async () => {
    if (!params.subjectId) return;
    setLoading(true);
    try {
      const res = await ledgerApi.detail(params);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const columns = [
    { title: '日期', dataIndex: 'voucherDate', key: 'voucherDate' },
    { title: '凭证号', dataIndex: 'voucherNo', key: 'voucherNo' },
    { title: '摘要', dataIndex: 'summary', key: 'summary', ellipsis: true },
    { title: '借方金额', dataIndex: 'debit', key: 'debit', align: 'right' },
    { title: '贷方金额', dataIndex: 'credit', key: 'credit', align: 'right' },
    { title: '方向', dataIndex: 'direction', key: 'direction' },
    { title: '余额', dataIndex: 'balance', key: 'balance', align: 'right' },
  ];

  return (
    <Card title="明细账">
      <Space style={{ marginBottom: 16 }}>
        <Select allowClear placeholder="选择科目（必填）" style={{ width: 200 }} value={params.subjectId} onChange={(v) => setParams({ ...params, subjectId: v })} options={subjects.map(s => ({ label: `${s.subjectCode} ${s.subjectName}`, value: s.id }))} />
        <Select value={params.startPeriod} onChange={(v) => setParams({ ...params, startPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <span>至</span>
        <Select value={params.endPeriod} onChange={(v) => setParams({ ...params, endPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <Button type="primary" onClick={loadData} disabled={!params.subjectId}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} scroll={{ y: 500 }} />
    </Card>
  );
};

export default DetailLedger;
