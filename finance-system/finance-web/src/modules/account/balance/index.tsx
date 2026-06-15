import React, { useState, useEffect } from 'react';
import { Card, Table, InputNumber, Button, message, Space } from 'antd';
import { balanceApi, subjectApi } from '@/api/account';
import { Select } from 'antd';
import type { BalanceItem, Subject } from '@/types/account.d';
import dayjs from 'dayjs';

/** 期初余额管理 */
const BalanceList: React.FC = () => {
  const [yearPeriod, setYearPeriod] = useState(dayjs().format('YYYY-01'));
  const [data, setData] = useState<BalanceItem[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadSubjects();
    loadData();
  }, [yearPeriod]);

  const loadSubjects = async () => {
    const res = await subjectApi.list();
    setSubjects(res.data || []);
  };

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await balanceApi.list(yearPeriod);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const handleChange = (id: number, field: 'debit' | 'credit', value: number | null) => {
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
      title: '期初借方', dataIndex: 'debit', key: 'debit', align: 'right',
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'debit', v)} style={{ width: 120 }} />
      ),
    },
    {
      title: '期初贷方', dataIndex: 'credit', key: 'credit', align: 'right',
      render: (val: number, record: BalanceItem) => (
        <InputNumber value={val} onChange={(v) => handleChange(record.id, 'credit', v)} style={{ width: 120 }} />
      ),
    },
  ];

  return (
    <Card title="期初余额" extra={<Button type="primary" onClick={handleSave} loading={loading}>保存</Button>}>
      <Space style={{ marginBottom: 16 }}>
        <span>会计期间：</span>
        <Select value={yearPeriod} onChange={setYearPeriod} style={{ width: 120 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} />
    </Card>
  );
};

export default BalanceList;
