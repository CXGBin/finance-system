import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Space, Button, Empty, message } from 'antd';
import { ledgerApi } from '@/api/account';
import { subjectApi } from '@/api/account';
import type { Subject, LedgerRecord } from '@/types/account.d';
import dayjs from 'dayjs';

/** 总账页面 */
const GeneralLedger: React.FC = () => {
  const [data, setData] = useState<LedgerRecord[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [params, setParams] = useState({ subjectId: undefined as number | undefined, startPeriod: dayjs().startOf('year').format('YYYY-MM'), endPeriod: dayjs().format('YYYY-MM') });

  useEffect(() => { loadSubjects(); loadData(); }, []);

  const loadSubjects = async () => {
    try {
      const res = await subjectApi.list();
      setSubjects(res.data || []);
    } catch {
      message.error('加载科目列表失败');
    }
  };

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await ledgerApi.general(params);
      setData(res.data || []);
    } catch {
      setError('查询总账数据失败');
      message.error('查询总账数据失败');
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '期间', dataIndex: 'period', key: 'period' },
    { title: '期初借方', dataIndex: 'openingDebit', key: 'openingDebit', align: 'right' },
    { title: '期初贷方', dataIndex: 'openingCredit', key: 'openingCredit', align: 'right' },
    { title: '本期借方发生', dataIndex: 'currentDebit', key: 'currentDebit', align: 'right' },
    { title: '本期贷方发生', dataIndex: 'currentCredit', key: 'currentCredit', align: 'right' },
    { title: '期末借方', dataIndex: 'closingDebit', key: 'closingDebit', align: 'right' },
    { title: '期末贷方', dataIndex: 'closingCredit', key: 'closingCredit', align: 'right' },
  ];

  return (
    <Card title="总账">
      <Space style={{ marginBottom: 16 }}>
        <Select allowClear placeholder="选择科目" style={{ width: 200 }} value={params.subjectId} onChange={(v) => setParams({ ...params, subjectId: v })} options={subjects.map(s => ({ label: `${s.subjectCode} ${s.subjectName}`, value: s.id }))} />
        <span>期间：</span>
        <Select value={params.startPeriod} onChange={(v) => setParams({ ...params, startPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <span>至</span>
        <Select value={params.endPeriod} onChange={(v) => setParams({ ...params, endPeriod: v })} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
        <Button type="primary" onClick={loadData}>查询</Button>
      </Space>
      {error ? (
        <Empty description={error} />
      ) : (
        <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false}
          locale={{ emptyText: <Empty description="暂无总账数据，请选择科目和期间后查询" /> }}
        />
      )}
    </Card>
  );
};

export default GeneralLedger;
