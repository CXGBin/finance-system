import React, { useState, useEffect } from 'react';
import { Card, Table, InputNumber, Button, message, Select, Space, Statistic, Row, Col, Spin, Alert, Empty } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { balanceApi, subjectApi, periodApi } from '@/api/account';
import type { BalanceItem, AccountingPeriod } from '@/types/account.d';

/** 期初余额管理 */
const BalanceList: React.FC = () => {
  const [selectedPeriodId, setSelectedPeriodId] = useState<number | undefined>();
  const [periods, setPeriods] = useState<AccountingPeriod[]>([]);
  const [data, setData] = useState<BalanceItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => { loadPeriods(); }, []);

  useEffect(() => { if (selectedPeriodId) loadData(); }, [selectedPeriodId]);

  const loadPeriods = async () => {
    try {
      const year = new Date().getFullYear();
      const res = await periodApi.list(year);
      const list = res.data || [];
      setPeriods(list);
      if (list.length > 0) setSelectedPeriodId(list[0].id);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载会计期间失败');
    }
  };

  const loadData = async () => {
    if (!selectedPeriodId) return;
    setLoading(true);
    setError(null);
    try {
      const res = await balanceApi.list(selectedPeriodId);
      setData(res.data || []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载余额数据失败');
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
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={() => selectedPeriodId && loadData()}>重试</Button>} />
        ) : (
          <Table columns={columns} dataSource={data} rowKey="id" pagination={false} size="middle" locale={{ emptyText: <Empty description="暂无余额数据" /> }} />
        )}
      </Spin>
    </Card>
  );
};

export default BalanceList;
