import React, { useState, useEffect } from 'react';
import { Card, Table, InputNumber, Button, message, Select, Space, Statistic, Row, Col } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { balanceApi, subjectApi, periodApi } from '@/api/account';
import type { BalanceItem, AccountingPeriod } from '@/types/account.d';

/** 期初余额管理 */
const BalanceList: React.FC = () => {
  const [selectedPeriodId, setSelectedPeriodId] = useState<number | undefined>();
  const [periods, setPeriods] = useState<AccountingPeriod[]>([]);
  const [data, setData] = useState<BalanceItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => { loadPeriods(); }, []);

  useEffect(() => { if (selectedPeriodId) loadData(); }, [selectedPeriodId]);

  const loadPeriods = async () => {
    const year = new Date().getFullYear();
    try {
      const res = await periodApi.list(year);
      const list = res.data || [];
      setPeriods(list);
      if (list.length > 0) setSelectedPeriodId(list[0].id);
    } catch { /* ignore */ }
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
    setSaving(true);
    try {
      await balanceApi.save(data);
      message.success('保存成功');
    } finally { setSaving(false); }
  };

  const debitTotal = data.reduce((sum, item) => sum + (item.beginDebit || 0), 0);
  const creditTotal = data.reduce((sum, item) => sum + (item.beginCredit || 0), 0);

  const columns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode', width: 120 },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName', ellipsis: true },
    {
      title: '期初借方', dataIndex: 'beginDebit', key: 'beginDebit', align: 'right', width: 180,
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'beginDebit', v)} style={{ width: '100%' }} precision={2} />
      ),
    },
    {
      title: '期初贷方', dataIndex: 'beginCredit', key: 'beginCredit', align: 'right', width: 180,
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'beginCredit', v)} style={{ width: '100%' }} precision={2} />
      ),
    },
  ];

  return (
    <Card title="期初余额" extra={
      <Space>
        <Select value={selectedPeriodId} onChange={setSelectedPeriodId} style={{ width: 200 }}
          placeholder="选择会计期间"
          options={periods.map(p => ({ label: `${p.periodYear}-${String(p.periodMonth).padStart(2, '0')}`, value: p.id }))} />
        <Button type="primary" icon={<SaveOutlined />} onClick={handleSave} loading={saving}>保存</Button>
      </Space>
    }>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={8}><Statistic title="期初借方合计" value={debitTotal} precision={2} /></Col>
        <Col span={8}><Statistic title="期初贷方合计" value={creditTotal} precision={2} /></Col>
        <Col span={8}><Statistic title="差额" value={debitTotal - creditTotal} precision={2} valueStyle={{ color: Math.abs(debitTotal - creditTotal) < 0.01 ? '#3f8600' : '#cf1322' }} /></Col>
      </Row>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} size="middle" />
    </Card>
  );
};

export default BalanceList;
