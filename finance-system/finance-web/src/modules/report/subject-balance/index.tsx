import React, { useState } from 'react';
import { Card, Select, Space, Button, Switch, InputNumber, message } from 'antd';
import { FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { reportApi, reportExportApi } from '@/api/report';
import dayjs from 'dayjs';

/** 科目余额表 */
const SubjectBalanceReport: React.FC = () => {
  const [period, setPeriod] = useState(dayjs().format('YYYY-MM'));
  const [showZero, setShowZero] = useState(false);
  const [subjectType, setSubjectType] = useState<number | undefined>();
  const [level, setLevel] = useState<number | undefined>();

  const columns: ProColumns<any>[] = [
    { title: '科目编码', dataIndex: 'subjectCode', search: false, width: 120 },
    { title: '科目名称', dataIndex: 'subjectName', ellipsis: true, search: false },
    { title: '期初借方', dataIndex: 'beginDebit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.beginDebit ?? 0).toFixed(2)}</span> },
    { title: '期初贷方', dataIndex: 'beginCredit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.beginCredit ?? 0).toFixed(2)}</span> },
    { title: '本期借方', dataIndex: 'currentDebit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.currentDebit ?? 0).toFixed(2)}</span> },
    { title: '本期贷方', dataIndex: 'currentCredit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.currentCredit ?? 0).toFixed(2)}</span> },
    { title: '期末借方', dataIndex: 'endDebit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.endDebit ?? 0).toFixed(2)}</span> },
    { title: '期末贷方', dataIndex: 'endCredit', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.endCredit ?? 0).toFixed(2)}</span> },
  ];

  return (
    <Card title="科目余额表" extra={
      <Space>
        <Select value={period} onChange={setPeriod} style={{ width: 140 }}
          options={Array.from({ length: 12 }, (_, i) => ({ label: dayjs().subtract(11 - i, 'month').format('YYYY-MM'), value: dayjs().subtract(11 - i, 'month').format('YYYY-MM') }))} />
        <Select value={subjectType} onChange={setSubjectType} allowClear placeholder="科目类型" style={{ width: 120 }}
          options={[{ label: '全部', value: undefined }, { label: '资产', value: 1 }, { label: '负债', value: 2 }, { label: '权益', value: 3 }, { label: '成本', value: 4 }, { label: '损益', value: 5 }]} />
        <Select value={level} onChange={setLevel} allowClear placeholder="级次" style={{ width: 100 }}
          options={[{ label: '全部', value: undefined }, { label: '1级', value: 1 }, { label: '2级', value: 2 }, { label: '3级', value: 3 }]} />
        <Switch size="small" checked={showZero} onChange={setShowZero} checkedChildren="显示零值" unCheckedChildren="隐藏零值" />
        <Button icon={<FileExcelOutlined />} onClick={() => reportExportApi.exportExcel('subject-balance', period)}>导出</Button>
      </Space>
    }>
      <ProTable<any>
        headerTitle=""
        rowKey={(r) => r.subjectCode}
        columns={columns}
        search={false}
        request={async () => {
          const res = await reportApi.subjectBalance(period, showZero, subjectType, level);
          const list = res.data?.items ?? res.data ?? [];
          return { data: list, success: true, total: list.length };
        }}
        pagination={false}
      />
    </Card>
  );
};

export default SubjectBalanceReport;
