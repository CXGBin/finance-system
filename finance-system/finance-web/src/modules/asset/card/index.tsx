import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, message } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { assetApi } from '@/api/asset';
import { useNavigate } from 'react-router-dom';

/** 资产卡片管理 */
const AssetCard: React.FC = () => {
  const navigate = useNavigate();
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();
  const actionRef = useRef<ActionType>();

  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '资产编号', dataIndex: 'assetCode', search: true, sorter: true },
    { title: '资产名称', dataIndex: 'assetName', search: true },
    { title: '原值', dataIndex: 'originalValue', align: 'right', sorter: true },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', align: 'right', sorter: true },
    { title: '净值', dataIndex: 'netValue', align: 'right', sorter: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 1: { text: '在用' }, 2: { text: '闲置' }, 3: { text: '维修中' }, 4: { text: '已处置' }, 5: { text: '已报废' } }, search: true },
    { title: '存放地点', dataIndex: 'location', search: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => <a onClick={() => navigate(`/asset/card/${(record as any).id}`)}>详情</a>,
    },
  ];

  const handleAdd = () => { form.resetFields(); setModalOpen(true); };
  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await assetApi.cardAdd({ ...values, acquisitionDate: values.acquisitionDate?.format('YYYY-MM-DD') } as any);
      message.success('新增成功');
      setModalOpen(false);
      actionRef.current?.reload();
    } catch {}
  };

  const request = createProTableRequest((params) => assetApi.cardList(params));

  return (
    <div>
      <ProTable
        actionRef={actionRef}
        columns={columns}
        request={request}
        search={{ labelWidth: 'auto' }}
        rowKey="id"
        toolBarRender={() => [<a key="add" onClick={handleAdd}>新增资产</a>]}
      />
      <Modal title="新增资产" open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="assetName" label="资产名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="categoryId" label="资产分类" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="originalValue" label="资产原值" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="acquisitionDate" label="入账日期"><DatePicker /></Form.Item>
          <Form.Item name="usefulLifeMonths" label="使用年限（月）"><InputNumber /></Form.Item>
          <Form.Item name="location" label="存放地点"><Input /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default AssetCard;
