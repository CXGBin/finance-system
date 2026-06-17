import React, { useState, useEffect } from 'react';
import { Card, Form, Input, DatePicker, Select, Table, Button, Space, InputNumber, message, Spin, Alert } from 'antd';
import { useNavigate } from 'react-router-dom';
import { voucherApi, subjectApi } from '@/api/account';
import type { Subject, VoucherEntry } from '@/types/account.d';
import dayjs from 'dayjs';

/** 凭证新增/编辑页面 */
const VoucherAdd: React.FC = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [subjectsLoading, setSubjectsLoading] = useState(false);
  const [subjectsError, setSubjectsError] = useState<string | null>(null);
  const [entries, setEntries] = useState<VoucherEntry[]>([{ id: Date.now(), subjectId: 0, summary: '', debitAmount: 0, creditAmount: 0 } as VoucherEntry]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadSubjects(); }, []);

  const loadSubjects = async () => {
    setSubjectsLoading(true);
    setSubjectsError(null);
    try {
      const res = await subjectApi.list();
      setSubjects(res.data || []);
    } catch (err: unknown) {
      setSubjectsError(err instanceof Error ? err.message : '加载科目列表失败');
    } finally { setSubjectsLoading(false); }
  };

  const addEntry = () => {
    setEntries([...entries, { id: Date.now(), subjectId: 0, summary: '', debitAmount: 0, creditAmount: 0 } as VoucherEntry]);
  };

  const removeEntry = (rowId: number) => {
    if (entries.length <= 2) { message.warning('至少保留两条分录'); return; }
    setEntries(entries.filter(e => e.id !== rowId));
  };

  const updateEntry = (rowId: number, field: string, value: string | number) => {
    setEntries(entries.map(e => e.id === rowId ? { ...e, [field]: value } : e));
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (entries.some(e => !e.subjectId)) { message.warning('分录科目不能为空'); return; }
    const totalDebit = entries.reduce((s, e) => s + (e.debitAmount || 0), 0);
    const totalCredit = entries.reduce((s, e) => s + (e.creditAmount || 0), 0);
    if (Math.abs(totalDebit - totalCredit) > 0.01) { message.warning(`借贷不平：借方${totalDebit} ≠ 贷方${totalCredit}`); return; }

    setLoading(true);
    try {
      await voucherApi.add({
        voucherDate: values.voucherDate?.format('YYYY-MM-DD'),
        abstractText: values.summary,
        voucherType: 0,
        entries: entries.map(e => ({
          subjectId: e.subjectId,
          summary: e.summary,
          debitAmount: e.debitAmount || 0,
          creditAmount: e.creditAmount || 0,
        })),
      });
      message.success('保存成功');
      navigate('/account/voucher');
    } finally { setLoading(false); }
  };

  const entryColumns = [
    { title: '科目', dataIndex: 'subjectId', render: (val: number, record: VoucherEntry) => (
      <Spin spinning={subjectsLoading} size="small">
        {subjectsError ? <Alert type="error" message={subjectsError} banner /> : (
          <Select style={{ width: 200 }} value={val} onChange={(v) => updateEntry(record.id, 'subjectId', v)}
            showSearch optionFilterProp="label" options={subjects.map(s => ({ label: `${s.subjectCode} ${s.subjectName}`, value: s.id }))} />
        )}
      </Spin>), }
    { title: '摘要', dataIndex: 'summary', render: (val: string, record: VoucherEntry) => (
      <Input value={val} onChange={(e) => updateEntry(record.id, 'summary', e.target.value)} style={{ width: 150 }} />
    )},
    { title: '借方', dataIndex: 'debitAmount', align: 'right', render: (val: number, record: VoucherEntry) => (
      <InputNumber value={val} onChange={(v) => updateEntry(record.id, 'debitAmount', v)} style={{ width: 120 }} min={0} />
    )},
    { title: '贷方', dataIndex: 'creditAmount', align: 'right', render: (val: number, record: VoucherEntry) => (
      <InputNumber value={val} onChange={(v) => updateEntry(record.id, 'creditAmount', v)} style={{ width: 120 }} min={0} />
    )},
    { title: '操作', render: (_: unknown, record: VoucherEntry) => <Button type="link" danger onClick={() => removeEntry(record.id)}>删除</Button> },
  ];

  const totalDebit = entries.reduce((s, e) => s + (e.debitAmount || 0), 0);
  const totalCredit = entries.reduce((s, e) => s + (e.creditAmount || 0), 0);

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
