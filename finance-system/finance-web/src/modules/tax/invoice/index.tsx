import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, Select, message } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { taxApi } from '@/api/tax';

/** 发票登记 */
const TaxInvoice: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();
  const actionRef = useRef<ActionType>();

  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '发票号码', dataIndex: 'invoiceNo', search: true, sorter: true },
    { title: '类型', dataIndex: 'invoiceType', valueType: 'select', valueEnum: { 1: { text: '专票' }, 2: { text: '普票' }, 3: { text: '其他' } }, search: true },
    { title: '方向', dataIndex: 'direction', valueType: 'select', valueEnum: { 1: { text: '进项' }, 2: { text: '销项' } }, search: true },
    { title: '对方名称', dataIndex: 'counterpartyName', search: true, ellipsis: true },
    { title: '税额', dataIndex: 'taxAmount', align: 'right', sorter: true },
    { title: '价税合计', dataIndex: 'totalAmount', align: 'right', sorter: true },
    { title: '已认证', dataIndex: 'isVerified', valueType: 'select', valueEnum: { 0: { text: '未认证' }, 1: { text: '已认证' } }, search: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => {
        const r = record as any;
        return r.isVerified === 0 ? <a style={{ color: '#ff4d4f' }} onClick={() => { taxApi.invoiceRemove(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>删除</a> : null;
      },
    },
  ];

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await taxApi.invoiceAdd({ ...values, invoiceDate: values.invoiceDate?.format('YYYY-MM-DD') } as any);
      message.success('登记成功');
      setModalOpen(false);
      actionRef.current?.reload();
    } catch {}
  };

  const request = createProTableRequest((params) => taxApi.invoiceList(params));

  return (
    <div>
      <ProTable
        actionRef={actionRef}
        columns={columns}
        request={request}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        rowKey="id"
        toolBarRender={() => [<a key="add" onClick={() => { form.resetFields(); setModalOpen(true); }}>登记发票</a>]}
      />
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
