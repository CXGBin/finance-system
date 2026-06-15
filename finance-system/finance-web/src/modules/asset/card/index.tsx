import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, Select, message } from 'antd';
import ProTable from '@/components/ProTable';
import { assetApi } from '@/api/asset';
import { useNavigate } from 'react-router-dom';
import type { AssetCard } from '@/types/asset.d';

/** 资产卡片管理 */
const AssetCard: React.FC = () => {
  const navigate = useNavigate();
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const columns = [
    { title: '资产编号', dataIndex: 'assetCode', key: 'assetCode', search: true },
    { title: '资产名称', dataIndex: 'assetName', key: 'assetName', search: true },
    { title: '原值', dataIndex: 'originalValue', key: 'originalValue', align: 'right' },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', key: 'accumulatedDepreciation', align: 'right' },
    { title: '净值', dataIndex: 'netValue', key: 'netValue', align: 'right' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => <span>{['', '在用', '闲置', '维修中', '已处置', '已报废'][val] || '未知'}</span>,
    },
    { title: '操作', key: 'action', render: (_: unknown, record: AssetCard) => <a onClick={() => navigate(`/asset/card/${record.id}`)}>详情</a> },
  ];

  const handleAdd = () => { form.resetFields(); setModalOpen(true); };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await assetApi.cardAdd({ ...values, acquisitionDate: values.acquisitionDate?.format('YYYY-MM-DD') } as any);
      message.success('新增成功');
      setModalOpen(false);
    } catch {}
  };

  return (
    <div>
      <ProTable columns={columns} fetchData={(params) => assetApi.cardList(params as any)} toolbarActions={<a onClick={handleAdd}>新增资产</a>} />
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
