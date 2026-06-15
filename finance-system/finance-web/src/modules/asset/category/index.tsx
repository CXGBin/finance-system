import React, { useState, useEffect } from 'react';
import { Tree, Button, Modal, Form, Input, InputNumber, Select, message, Popconfirm } from 'antd';
import { assetApi } from '@/api/asset';
import type { AssetCategory } from '@/types/asset.d';

/** 资产分类管理 */
const AssetCategory: React.FC = () => {
  const [treeData, setTreeData] = useState<AssetCategory[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<AssetCategory | null>(null);
  const [form] = Form.useForm();

  useEffect(() => { loadData(); }, []);

  const loadData = async () => {
    const res = await assetApi.categoryTree();
    setTreeData(res.data || []);
  };

  const handleAdd = (parent?: AssetCategory) => {
    setEditingRecord(null);
    form.resetFields();
    if (parent) form.setFieldsValue({ parentId: parent.id });
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await assetApi.categoryUpdate({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await assetApi.categoryAdd(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadData();
    } catch {}
  };

  const buildNodes = (list: AssetCategory[]): DataNode[] => list.map(item => ({
    key: item.id, title: <span>{item.categoryName} <Button type="link" size="small" onClick={(e) => { e.stopPropagation(); handleAdd(item); }}>新增</Button></span>,
    children: item.children ? buildNodes(item.children) : [],
  }));

  return (
    <div>
      <Button type="primary" onClick={() => handleAdd()} style={{ marginBottom: 16 }}>新增分类</Button>
      <Tree showLine defaultExpandAll treeData={buildNodes(treeData)} />
      <Modal title={editingRecord ? '编辑分类' : '新增分类'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="父级" hidden><Input /></Form.Item>
          <Form.Item name="categoryCode" label="编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="categoryName" label="名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="depreciationMethod" label="折旧方法"><Select options={[{ label: '直线法', value: 1 }, { label: '双倍余额递减法', value: 2 }, { label: '年数总和法', value: 3 }]} /></Form.Item>
          <Form.Item name="usefulLifeMonths" label="使用年限（月）"><InputNumber /></Form.Item>
          <Form.Item name="residualRate" label="残值率(%)"><InputNumber /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default AssetCategory;
