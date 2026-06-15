import React, { useState, useEffect } from 'react';
import { Card, Form, Input, DatePicker, Select, Table, Button, Space, InputNumber, message } from 'antd';
import { useNavigate } from 'react-router-dom';
import { voucherApi, subjectApi } from '@/api/account';
import type { Subject } from '@/types/account.d';
import dayjs from 'dayjs';

/** 凭证新增/编辑页面 */
const VoucherAdd: React.FC = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [entries, setEntries] = useState<any[]>([{ id: Date.now(), subjectId: undefined, summary: '', debit: 0, credit: 0 }]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadSubjects(); }, []);

  const loadSubjects = async () => {
    const res = await subjectApi.list();
    setSubjects(res.data || []);
  };

  const addEntry = () => {
    setEntries([...entries, { id: Date.now(), subjectId: undefined, summary: '', debit: 0, credit: 0 }]);
  };

  const removeEntry = (rowId: number) => {
    if (entries.length <= 2) { message.warning('至少保留两条分录'); return; }
    setEntries(entries.filter(e => e.id !== rowId));
  };

  const updateEntry = (rowId: number, field: string, value: any) => {
    setEntries(entries.map(e => e.id === rowId ? { ...e, [field]: value } : e));
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (entries.some(e => !e.subjectId)) { message.warning('分录科目不能为空'); return; }
    const totalDebit = entries.reduce((s, e) => s + (e.debit || 0), 0);
    const totalCredit = entries.reduce((s, e) => s + (e.credit || 0), 0);
    if (Math.abs(totalDebit - totalCredit) > 0.01) { message.warning(`借贷不平：借方${totalDebit} ≠ 贷方${totalCredit}`); return; }

    setLoading(true);
    try {
      await voucherApi.add({
        voucherDate: values.voucherDate?.format('YYYY-MM-DD'),
        summary: values.summary,
        periodId: values.periodId,
        entries: entries.map(e => ({
          subjectId: e.subjectId,
          summary: e.summary,
          debit: e.debit,
          credit: e.credit,
        })),
      } as any);
      message.success('保存成功');
      navigate('/account/voucher');
    } finally { setLoading(false); }
  };

  const entryColumns = [
    { title: '科目', dataIndex: 'subjectId', render: (val: number, record: any) => (
      <Select style={{ width: 200 }} value={val} onChange={(v) => updateEntry(record.id, 'subjectId', v)}
        showSearch optionFilterProp="label" options={subjects.map(s => ({ label: `${s.subjectCode} ${s.subjectName}`, value: s.id }))} />
    )},
    { title: '摘要', dataIndex: 'summary', render: (val: string, record: any) => (
      <Input value={val} onChange={(e) => updateEntry(record.id, 'summary', e.target.value)} style={{ width: 150 }} />
    )},
    { title: '借方', dataIndex: 'debit', align: 'right', render: (val: number, record: any) => (
      <InputNumber value={val} onChange={(v) => updateEntry(record.id, 'debit', v)} style={{ width: 120 }} min={0} />
    )},
    { title: '贷方', dataIndex: 'credit', align: 'right', render: (val: number, record: any) => (
      <InputNumber value={val} onChange={(v) => updateEntry(record.id, 'credit', v)} style={{ width: 120 }} min={0} />
    )},
    { title: '操作', render: (_: any, record: any) => <Button type="link" danger onClick={() => removeEntry(record.id)}>删除</Button> },
  ];

  const totalDebit = entries.reduce((s, e) => s + (e.debit || 0), 0);
  const totalCredit = entries.reduce((s, e) => s + (e.credit || 0), 0);

  return (
    <Card title="新增凭证">
      <Form form={form} layout="inline" style={{ marginBottom: 16 }}>
        <Form.Item name="voucherDate" label="凭证日期" rules={[{ required: true }]}>
          <DatePicker defaultValue={dayjs()} />
        </Form.Item>
        <Form.Item name="summary" label="摘要" rules={[{ required: true }]}>
          <Input style={{ width: 300 }} />
        </Form.Item>
      </Form>
      <Table columns={entryColumns} dataSource={entries} rowKey="id" pagination={false} footer={() => (
        <Space style={{ float: 'right' }}>
          <span>借方合计: {totalDebit.toFixed(2)}</span>
          <span>贷方合计: {totalCredit.toFixed(2)}</span>
          <span style={{ color: Math.abs(totalDebit - totalCredit) < 0.01 ? '#52c41a' : '#ff4d4f' }}>差额: {(totalDebit - totalCredit).toFixed(2)}</span>
        </Space>
      )} />
      <div style={{ marginTop: 16 }}>
        <Space>
          <Button onClick={addEntry}>添加分录</Button>
          <Button type="primary" onClick={handleSubmit} loading={loading}>保存</Button>
          <Button onClick={() => navigate('/account/voucher')}>取消</Button>
        </Space>
      </div>
    </Card>
  );
};

export default VoucherAdd;
