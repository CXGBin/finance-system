import React, { useState } from 'react';
import { Modal, Form, Input, TreeSelect, message, Popconfirm, Space } from 'antd';
import type { SysRole } from '@/types/system.d';
import { roleApi, menuApi } from '@/api/system';
import ProTable from '@/components/ProTable';

/** 角色管理页面 */
const RoleList: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [permModalOpen, setPermModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<SysRole | null>(null);
  const [currentRole, setCurrentRole] = useState<SysRole | null>(null);
  const [menuTree, setMenuTree] = useState<any[]>([]);
  const [checkedKeys, setCheckedKeys] = useState<number[]>([]);
  const [form] = Form.useForm();

  const columns = [
    { title: '角色名称', dataIndex: 'roleName', key: 'roleName', search: true },
    { title: '角色标识', dataIndex: 'roleKey', key: 'roleKey' },
    { title: '排序', dataIndex: 'sortOrder', key: 'sortOrder' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => (
        <span style={{ color: val === 1 ? '#52c41a' : '#ff4d4f' }}>{val === 1 ? '正常' : '停用'}</span>
      ),
    },
    { title: '创建时间', dataIndex: 'createdTime', key: 'createdTime' },
    {
      title: '操作', key: 'action',
      render: (_: any, record: SysRole) => (
        <Space>
          <a className="table-action" onClick={() => handleEdit(record as SysRole)}>编辑</a>
          <a className="table-action" onClick={() => handlePerm(record as SysRole)}>权限</a>
          <Popconfirm title="确认删除该角色?" onConfirm={() => handleDelete(record)}>
            <a className="table-action" style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleEdit = (record: SysRole) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await roleApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await roleApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
    } catch {}
  };

  /** 打开权限配置 */
  const handlePerm = async (record: SysRole) => {
    setCurrentRole(record);
    // 获取菜单树
    const menuRes = await menuApi.tree();
    // 获取角色已有菜单权限ID列表
    const menusRes = await roleApi.menus(record.id);
 setMenuTree(menuRes.data || []);
    setCheckedKeys(menusRes.data || []);
    setPermModalOpen(true);
  };

  /** 保存权限配置 */
  const handleSavePerm = async () => {
    if (!currentRole) return;
    await roleApi.update({ ...currentRole, menuIds: checkedKeys } as Partial<SysRole>);
    message.success('权限配置已保存');
    setPermModalOpen(false);
  };

  /** 删除角色 */
  const handleDelete = async (record: SysRole) => {
    await roleApi.remove(record.id);
    message.success('删除成功');
  };

  return (
    <div>
      <ProTable
        columns={columns}
        fetchData={(params) => roleApi.list(params as any)}
        toolbarActions={<a onClick={handleAdd}>新增角色</a>}
      />
      <Modal title={editingRecord ? '编辑角色' : '新增角色'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="roleName" label="角色名称" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="roleKey" label="角色标识" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="sortOrder" label="排序" initialValue={0}>
            <Input type="number" />
          </Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked" initialValue={true}>
            <Input type="number" />
          </Form.Item>
        </Form>
      </Modal>
      <Modal title={`权限配置 - ${currentRole?.roleName}`} open={permModalOpen} onOk={handleSavePerm} onCancel={() => setPermModalOpen(false)} width={500}>
        <TreeSelect
          treeData={menuTree}
          value={checkedKeys}
          onChange={(keys) => setCheckedKeys(keys as number[])}
          treeCheckable
          style={{ width: '100%' }}
          placeholder="选择菜单权限"
        />
      </Modal>
    </div>
  );
};

export default RoleList;
