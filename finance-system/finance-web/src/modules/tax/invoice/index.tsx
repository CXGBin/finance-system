import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, Select, Button, message, Space, Tag } from 'antd';
import ProTable, { type ProTableRef } from '@/components/ProTable';
import { taxApi } from '@/api/tax';
import type { Invoice } from '@/types/tax.d';

/** 发票登记 */
const TaxInvoice: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();
  const actionRef = useRef<ProTableRef>(null);

  const columns = [
    { title: '发票号码', dataIndex: 'invoiceNo', key: 'invoiceNo', search: true },
    { title: '类型', dataIndex: 'invoiceType', key: 'invoiceType', render: (v: number) => ['', '专票', '普票', '其他'][v] },
    { title: '方向', dataIndex: 'direction', key: 'direction', render: (v: number) => <Tag color={v === 1 ? 'blue' : 'green'}>{v === 1 ? '进项' : '销项'}</Tag> },
    { title: '对方名称', dataIndex: 'counterpartyName', key: 'counterpartyName', ellipsis: true },
    { title: '税额', dataIndex: 'taxAmount', key: 'taxAmount', align: 'right' },
    { title: '价税合计', dataIndex: 'totalAmount', key: 'totalAmount', align: 'right' },
    { title: '已认证', dataIndex: 'isVerified', key: 'isVerified', render: (v: number) => v === 1 ? <Tag color="success">已认证</Tag> : <Tag color="default">未认证</Tag> },
    {
      title: '操作', key: 'action', render: (_: unknown, record: Invoice) => (
        record.isVerified === 0 ? <a onClick={() => taxApi.invoiceRemove(record.id).then(() => actionRef.current?.refresh())} style={{ color: '#ff4d4f' }}>删除</a> : null
      ),
    },
  ];

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await taxApi.invoiceAdd({ ...values, invoiceDate: values.invoiceDate?.format('YYYY-MM-DD') } as any);
      message.success('登记成功');
      setModalOpen(false);
      actionRef.current?.refresh();
    } catch {}
  };

  return (
    <div>
      <ProTable ref={actionRef} columns={columns} fetchData={(params) => taxApi.invoiceList(params as any)} toolbarActions={<a onClick={() => { form.resetFields(); setModalOpen(true); }}>登记发票</a>} />
      <Modal title="登记发票" open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="invoiceType" label="发票类型" rules={[{ required: true }]}><Select options={[{ label: '增值税专用', value: 1 }, { label: '增值税普通', value: 2 }, { label: '其他', value: 3 }]} /></Form.Item>
          <Form.Item name="invoiceNo" label="发票号码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="invoiceDate" label="开票日期" rules={[{ required: true }]}><DatePicker /></Form.Item>
          <Form.Item name="direction" label="方向" rules={[{ required: true }]}><Select options={[{ label: '进项', value: 1 }, { label: '销项', value: 2 }]} /></Form.Item>
          <Form.Item name="counterpartyName" label="对方名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="taxAmount" label="税额" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="amountWithoutTax" label="不含税金额" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default TaxInvoice;
