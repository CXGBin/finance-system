import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, Select, message, Popconfirm, Space, Button, Tag } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import { taxApi } from '@/api/tax';
import type { TaxCategory } from '@/types/tax.d';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 税种管理 */
const TaxType: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TaxCategory | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<TaxCategory>[] = [
    { title: '税种编码', dataIndex: 'taxCode', ellipsis: true },
    { title: '税种名称', dataIndex: 'taxName', ellipsis: true },
    { title: '税率(%)', dataIndex: 'taxRate', sorter: true, search: false, align: 'right', render: (_, r) => `${(r.taxRate ?? 0).toFixed(2)}%` },
    { title: '计算方式', dataIndex: 'calculationMethod', valueType: 'select', search: false, valueEnum: { 1: { text: '从价计征' }, 2: { text: '从量计征' } } },
    { title: '申报周期', dataIndex: 'declareCycle', valueType: 'select', search: false, valueEnum: { 1: { text: '月报' }, 2: { text: '季报' }, 3: { text: '年报' } } },
    { title: '备注', dataIndex: 'remark', search: false, ellipsis: true },
    {
      title: '操作', valueType: 'option', width: 140,
      render: (_, record) => (
        <Space>
          <a onClick={() => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => { taxApi.remove(record.id); message.success('删除成功'); actionRef.current?.reload(); }}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <ProTable<TaxCategory> actionRef={actionRef} headerTitle="税种管理" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        request={createProTableRequest((params) => taxApi.typeList(params as any))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { setEditingRecord(null); form.resetFields(); form.setFieldsValue({ calculationMethod: 1, declareCycle: 1, taxRate: 0 }); setModalOpen(true); }}>新增税种</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal title={editingRecord ? '编辑税种' : '新增税种'} open={modalOpen} onOk={async () => {
        try {
          const values = await form.validateFields();
          if (editingRecord) { await taxApi.update(editingRecord.id, values); message.success('更新成功'); }
          else { await taxApi.add(values as any); message.success('新增成功'); }
          setModalOpen(false); actionRef.current?.reload();
        } catch { /* validation */ }
      }} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="taxCode" label="税种编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="taxName" label="税种名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="taxRate" label="税率(%)" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} min={0} precision={2} /></Form.Item>
          <Form.Item name="calculationMethod" label="计算方式" rules={[{ required: true }]}>
            <Select options={[{ label: '从价计征', value: 1 }, { label: '从量计征', value: 2 }]} />
          </Form.Item>
          <Form.Item name="declareCycle" label="申报周期" rules={[{ required: true }]}>
            <Select options={[{ label: '月报', value: 1 }, { label: '季报', value: 2 }, { label: '年报', value: 3 }]} />
          </Form.Item>
          <Form.Item name="remark" label="备注"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default TaxType;
