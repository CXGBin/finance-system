import React, { useState, useEffect } from 'react';
import { Modal, Form, Input, InputNumber, Select, Tree, message, Popconfirm } from 'antd';
import type { Menu } from '@/types/system.d';
import { menuApi } from '@/api/system';
import { Button } from 'antd';

/** 菜单管理页面 */
const MenuList: React.FC = () => {
  const [treeData, setTreeData] = useState<Menu[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Menu | null>(null);
  const [form] = Form.useForm();

  useEffect(() => { loadTree(); }, []);

  const loadTree = async () => {
    const res = await menuApi.tree();
    setTreeData(res.data || []);
  };

  const handleAdd = (parent?: Menu) => {
    setEditingRecord(null);
    form.resetFields();
    if (parent) form.setFieldsValue({ parentId: parent.id, parentName: parent.menuName });
    setModalOpen(true);
  };

  const handleEdit = (record: Menu) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await menuApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await menuApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadTree();
    } catch {}
  };

  const handleDelete = async (id: number) => {
    await menuApi.remove(id);
    message.success('删除成功');
    loadTree();
  };

  const buildTreeNodes = (list: Menu[]): DataNode[] =>
    list.map(item => ({
      key: item.id,
      title: (
        <span>
          {item.menuName}
          <Button type="link" size="small" onClick={(e) => { e.stopPropagation(); handleAdd(item); }}>新增</Button>
          <Button type="link" size="small" onClick={(e) => { e.stopPropagation(); handleEdit(item); }}>编辑</Button>
          <Popconfirm title="确认删除?" onConfirm={() => handleDelete(item.id)}>
            <Button type="link" size="small" danger onClick={(e) => e.stopPropagation()}>删除</Button>
          </Popconfirm>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : [],
    }));

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <Button type="primary" onClick={() => handleAdd()}>新增根菜单</Button>
      </div>
      <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} />
      <Modal title={editingRecord ? '编辑菜单' : '新增菜单'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="父菜单" hidden><Input /></Form.Item>
          <Form.Item name="menuName" label="菜单名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="path" label="路由地址"><Input /></Form.Item>
          <Form.Item name="component" label="组件路径"><Input /></Form.Item>
          <Form.Item name="icon" label="图标"><Input /></Form.Item>
          <Form.Item name="menuType" label="类型" rules={[{ required: true }]}>
            <Select options={[{ label: '目录', value: 1 }, { label: '菜单', value: 2 }, { label: '按钮', value: 3 }]} />
          </Form.Item>
          <Form.Item name="sortOrder" label="排序" initialValue={0}><InputNumber /></Form.Item>
          <Form.Item name="status" label="状态" initialValue={1}>
            <Select options={[{ label: '正常', value: 1 }, { label: '停用', value: 0 }]} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default MenuList;
