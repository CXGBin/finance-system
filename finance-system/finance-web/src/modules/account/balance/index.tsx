import React, { useState, useEffect } from 'react';
import { Card, Table, InputNumber, Button, message, Select, Space } from 'antd';
import { balanceApi, subjectApi, periodApi } from '@/api/account';
import type { BalanceItem, Subject, AccountingPeriod } from '@/types/account.d';

/** 期初余额管理 */
const BalanceList: React.FC = () => {
  const [selectedPeriodId, setSelectedPeriodId] = useState<number | undefined>();
  const [periods, setPeriods] = useState<AccountingPeriod[]>([]);
  const [data, setData] = useState<BalanceItem[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadPeriods();
    loadSubjects();
  }, []);

  useEffect(() => {
    if (selectedPeriodId) loadData();
  }, [selectedPeriodId]);

  const loadPeriods = async () => {
    const year = new Date().getFullYear();
    try {
      const res = await periodApi.list(year);
      const list = res.data || [];
      setPeriods(list);
      if (list.length > 0) setSelectedPeriodId(list[0].id);
    } catch {}
  };

  const loadSubjects = async () => {
    try {
      const res = await subjectApi.list();
      setSubjects(res.data || []);
    } catch {}
  };

  const loadData = async () => {
    if (!selectedPeriodId) return;
    setLoading(true);
    try {
      const res = await balanceApi.list(selectedPeriodId);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const handleChange = (id: number, field: 'beginDebit' | 'beginCredit', value: number | null) => {
    setData(prev => prev.map(item =>
      item.id === id ? { ...item, [field]: value || 0 } : item
    ));
  };

  const handleSave = async () => {
    setLoading(true);
    try {
      await balanceApi.save(data);
      message.success('保存成功');
    } finally { setLoading(false); }
  };

  const columns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    {
      title: '期初借方', dataIndex: 'beginDebit', key: 'beginDebit', align: 'right',
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'beginDebit', v)} style={{ width: 120 }} />
      ),
    },
    {
      title: '期初贷方', dataIndex: 'beginCredit', key: 'beginCredit', align: 'right',
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'beginCredit', v)} style={{ width: 120 }} />
      ),
    },
  ];

  return (
    <Card title="期初余额" extra={<Button type="primary" onClick={handleSave} loading={loading}>保存</Button>}>
      <Space style={{ marginBottom: 16 }}>
        <span>会计期间：</span>
        <Select
          value={selectedPeriodId}
          onChange={setSelectedPeriodId}
          style={{ width: 200 }}
          placeholder="选择会计期间"
          options={periods.map(p => ({ label: `${p.periodYear}-${String(p.periodMonth).padStart(2, '0')}`, value: p.id }))}
        />
        <Button onClick={loadData}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} />
    </Card>
  );
};

export default BalanceList;
