import React, { useState } from 'react';
import { Modal, Form, Input, message } from 'antd';
import ProTable from '@/components/ProTable';
import { approvalApi } from '@/api/approval';
import type { ApprovalTemplate } from '@/types/approval.d';

/** 审批模板管理 */
const ApprovalTemplate: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const columns = [
    { title: '模板名称', dataIndex: 'name', key: 'name', search: true },
    { title: '流程定义', dataIndex: 'flowName', key: 'flowName' },
    { title: '状态', dataIndex: 'status', key: 'status' },
    { title: '创建时间', dataIndex: 'createdTime', key: 'createdTime' },
  ];

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await approvalApi.templateAdd(values as any);
      message.success('保存成功');
      setModalOpen(false);
    } catch {}
  };

  return (
    <div>
      <ProTable
        columns={columns}
        fetchData={(params) => approvalApi.templateList(params as any)}
        toolbarActions={<a onClick={() => { form.resetFields(); setModalOpen(true); }}>新增模板</a>}
      />
      <Modal title="新增审批模板" open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="name" label="模板名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="flowId" label="关联流程"><Input /></Form.Item>
          <Form.Item name="description" label="描述"><Input.TextArea /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ApprovalTemplate;
