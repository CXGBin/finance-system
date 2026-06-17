import React, { useRef } from 'react';
import { Modal, Form, Input, Switch, message, Popconfirm, Space, Button, TreeSelect } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import type { SysRole } from '@/types/system.d';
import { roleApi } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 角色管理页面 */
const RoleList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [menuModalOpen, setMenuModalOpen] = React.useState(false);
  const [editingRole, setEditingRole] = React.useState<SysRole | null>(null);
  const [menuKeys, setMenuKeys] = React.useState<number[]>([]);
  const [form] = Form.useForm();

  const columns: ProColumns<SysRole>[] = [
    {
      title: '角色名称',
      dataIndex: 'roleName',
      ellipsis: true,
    },
    {
      title: '角色编码',
      dataIndex: 'roleCode',
      ellipsis: true,
    },
    {
      title: '状态',
      dataIndex: 'status',
      valueType: 'select',
      valueEnum: {
        1: { text: '启用', status: 'Success' },
        0: { text: '停用', status: 'Default' },
      },
      render: (_, record) => (
        <span>{record.status === 1 ? '启用' : '停用'}</span>
      ),
    },
    {
      title: '描述',
      dataIndex: 'description',
      ellipsis: true,
      search: false,
    },
    {
      title: '排序',
      dataIndex: 'sortOrder',
      sorter: true,
      search: false,
      width: 80,
    },
    {
      title: '创建时间',
      dataIndex: 'createdTime',
      valueType: 'dateTime',
      sorter: true,
      search: false,
      width: 180,
    },
    {
      title: '操作',
      valueType: 'option',
      width: 200,
      render: (_, record) => (
        <Space>
          <a onClick={() => handleEdit(record)}>编辑</a>
          <a onClick={() => handleAssignMenu(record)}>分配菜单</a>
          <Popconfirm title="确认删除该角色?" onConfirm={() => handleDelete(record)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => {
    setEditingRole(null);
    form.resetFields();
    form.setFieldsValue({ status: 1, sortOrder: 0 });
    setMenuModalOpen(true);
  };

  const handleEdit = (record: SysRole) => {
    setEditingRole(record);
    form.setFieldsValue(record);
    setMenuModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRole) {
        await roleApi.update({ ...editingRole, ...values });
        message.success('更新成功');
      } else {
        await roleApi.add(values as any);
        message.success('新增成功');
      }
      setMenuModalOpen(false);
      actionRef.current?.reload();
    } catch { /* form validation */ }
  };

  const handleDelete = async (record: SysRole) => {
    await roleApi.remove(record.id);
    message.success('删除成功');
    actionRef.current?.reload();
  };

  const handleAssignMenu = async (record: SysRole) => {
    const menus = await roleApi.menus(record.id);
    setEditingRole(record);
    setMenuKeys(menus);
  };

  const handleSaveMenus = async () => {
    if (editingRole) {
      await roleApi.saveMenus(editingRole.id, menuKeys);
      message.success('菜单分配成功');
      setEditingRole(null);
      setMenuKeys([]);
    }
  };

  return (
    <>
      <ProTable<SysRole>
        actionRef={actionRef}
        headerTitle="角色管理"
        rowKey="id"
        columns={columns}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        request={createProTableRequest((params) => roleApi.list(params as any))}
        toolBarRender={() => [
          <Button key="add" type="primary" icon={<PlusOutlined />} onClick={handleAdd}>新增角色</Button>,
        ]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal
        title={editingRole ? '编辑角色' : '新增角色'}
        open={menuModalOpen}
        onOk={handleSave}
        onCancel={() => setMenuModalOpen(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="roleName" label="角色名称" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="roleCode" label="角色编码" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="描述">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="sortOrder" label="排序">
            <Input type="number" />
          </Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked">
            <Switch checkedChildren="启用" unCheckedChildren="禁用" />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default RoleList;
