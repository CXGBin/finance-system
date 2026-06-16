import React, { useState, useEffect } from 'react';
import { Card, Modal, Form, Input, InputNumber, Select, message, Popconfirm, Tree, Space } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { AssetCategory } from '@/types/asset.d';
import { assetApi } from '@/api/asset';

/** 资产分类管理 */
const AssetCategoryPage: React.FC = () => {
  const [treeData, setTreeData] = useState<AssetCategory[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<AssetCategory | null>(null);
  const [form] = Form.useForm();

  const loadTree = async () => {
    const data = await assetApi.categoryTree();
    setTreeData(data.data ?? []);
  };

  useEffect(() => { loadTree(); }, []);

  const handleAdd = (parent?: AssetCategory) => {
    setEditingRecord(null);
    form.resetFields();
    form.setFieldsValue({ parentId: parent?.id ?? 0, depreciationMethod: 1, usefulLifeMonths: 60, residualRate: 5, sortOrder: 0 });
    setModalOpen(true);
  };

  const handleEdit = (record: AssetCategory) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) { await assetApi.categoryUpdate({ ...editingRecord, ...values }); message.success('更新成功'); }
      else { await assetApi.categoryAdd(values as any); message.success('新增成功'); }
      setModalOpen(false); loadTree();
    } catch { /* validation */ }
  };

  const handleDelete = async (record: AssetCategory) => {
    await assetApi.categoryRemove(record.id);
    message.success('删除成功');
    loadTree();
  };

  const depMethod = (m: number) => ({ 1: '平均年限法', 2: '双倍余额递减法', 3: '年数总和法' }[m] || '未知');

  const buildTreeNodes = (data: AssetCategory[]) =>
    data.map((item) => ({
      key: item.id,
      title: (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
          <span>{item.categoryCode}</span>
          <span>{item.categoryName}</span>
          <span style={{ color: '#999', fontSize: 12 }}>{depMethod(item.depreciationMethod)}</span>
          <Space size={4}>
            <a onClick={() => handleAdd(item)} style={{ fontSize: 12 }}><PlusOutlined /></a>
            <a onClick={() => handleEdit(item)} style={{ fontSize: 12 }}><EditOutlined /></a>
            <Popconfirm title="确认删除?" onConfirm={() => handleDelete(item)}>
              <a style={{ fontSize: 12, color: '#ff4d4f' }}><DeleteOutlined /></a>
            </Popconfirm>
          </Space>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : undefined,
    }));

  return (
    <>
      <Card title="资产分类" extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => handleAdd()}>新增分类</Button>}>
        {treeData.length > 0 ? (
          <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} blockNode />
        ) : (
          <div style={{ textAlign: 'center', padding: 40, color: '#999' }}>暂无分类数据</div>
        )}
      </Card>
      <Modal title={editingRecord ? '编辑分类' : '新增分类'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="上级分类" hidden><InputNumber /></Form.Item>
          <Form.Item name="categoryCode" label="分类编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="categoryName" label="分类名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="depreciationMethod" label="折旧方式" rules={[{ required: true }]}>
            <Select options={[{ label: '平均年限法', value: 1 }, { label: '双倍余额递减法', value: 2 }, { label: '年数总和法', value: 3 }]} />
          </Form.Item>
          <Form.Item name="usefulLifeMonths" label="使用年限(月)" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} min={1} /></Form.Item>
          <Form.Item name="residualRate" label="残值率(%)" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} min={0} max={100} precision={1} /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AssetCategoryPage;
