import React, { useState, useEffect } from 'react';
import { Tree, Button, Modal, Form, Input, InputNumber, Select, Tag, message, Popconfirm, Space } from 'antd';
import type { Subject } from '@/types/account.d';
import { subjectApi } from '@/api/account';

/** 科目管理页面 */
const SubjectList: React.FC = () => {
  const [treeData, setTreeData] = useState<Subject[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Subject | null>(null);
  const [form] = Form.useForm();

  useEffect(() => { loadTree(); }, []);

  const loadTree = async () => {
    const res = await subjectApi.tree();
    setTreeData(res.data || []);
  };

  const handleAdd = (parent?: Subject) => {
    setEditingRecord(null);
    form.resetFields();
    if (parent) form.setFieldsValue({ parentId: parent.id, parentName: parent.subjectName });
    setModalOpen(true);
  };

  const handleEdit = (record: Subject) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await subjectApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await subjectApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadTree();
    } catch {}
  };

  const handleDelete = async (id: number) => {
    await subjectApi.remove(id);
    message.success('删除成功');
    loadTree();
  };

  const buildTreeNodes = (list: Subject[]): any[] =>
    list.map(item => ({
      key: item.id,
      title: (
        <span>
          {item.subjectCode} {item.subjectName}
          {item.isEnabled === 0 && <Tag color="red" style={{ marginLeft: 4 }}>停用</Tag>}
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
        <Button type="primary" onClick={() => handleAdd()}>新增科目</Button>
      </div>
      <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} />
      <Modal title={editingRecord ? '编辑科目' : '新增科目'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="上级科目" hidden><Input /></Form.Item>
          <Form.Item name="subjectCode" label="科目编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="subjectName" label="科目名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="subjectType" label="科目类型" rules={[{ required: true }]}>
            <Select options={[{ label: '资产', value: 1 }, { label: '负债', value: 2 }, { label: '权益', value: 3 }, { label: '成本', value: 4 }, { label: '损益', value: 5 }]} />
          </Form.Item>
          <Form.Item name="balanceDirection" label="余额方向">
            <Select options={[{ label: '借方', value: 1 }, { label: '贷方', value: 2 }]} />
          </Form.Item>
          <Form.Item name="auxiliaryType" label="辅助核算">
            <Select allowClear options={[{ label: '无', value: '' }, { label: '客户', value: 'customer' }, { label: '供应商', value: 'supplier' }, { label: '项目', value: 'project' }]} />
          </Form.Item>
          <Form.Item name="isEnabled" label="状态" initialValue={1}><Input type="number" /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default SubjectList;
