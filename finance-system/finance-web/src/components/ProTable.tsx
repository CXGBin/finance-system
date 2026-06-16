/**
 * 兼容性 ProTable 包装器
 * 供尚未迁移到官方 ProTable 的旧页面使用
 * 新页面请直接使用 @ant-design/pro-components 的 ProTable
 */
import React, { useRef, useMemo, useImperativeHandle, forwardRef } from 'react';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import type { PageParams, SearchParams, PagedResult } from '@/types/api.d';

export interface ProTableRef {
  refresh: () => void;
}

export interface ProTableColumn<T = Record<string, unknown>> {
  title: string;
  dataIndex: string;
  key?: string;
  search?: boolean;
  searchRender?: React.ReactNode;
  sorter?: boolean;
  align?: 'left' | 'center' | 'right';
  width?: number;
  ellipsis?: boolean;
  render?: (val: unknown, record: T, index: number) => React.ReactNode;
  [key: string]: unknown;
}

interface LegacyProTableProps<T = Record<string, unknown>> {
  columns: ProTableColumn<T>[];
  fetchData: (params: PageParams & SearchParams) => Promise<PagedResult<T>>;
  rowKey?: string;
  toolbarActions?: React.ReactNode;
  searchInitialValues?: SearchParams;
  tableProps?: Record<string, unknown>;
  defaultPageSize?: number;
}

const LegacyProTableInner = forwardRef<ProTableRef, LegacyProTableProps>(function LegacyProTableInner<T extends Record<string, unknown>>({
  columns,
  fetchData,
  rowKey = 'id',
  toolbarActions,
  searchInitialValues = {},
  defaultPageSize = 10,
}, ref) {
  const actionRef = useRef<ActionType>();

  useImperativeHandle(ref, () => ({
    refresh: () => actionRef.current?.reload(),
  }), []);

  const proColumns: ProColumns<T>[] = useMemo(() =>
    columns.map((col) => {
      const proCol: ProColumns<T> = {
        title: col.title,
        dataIndex: col.dataIndex as string,
        key: col.key ?? col.dataIndex,
        ellipsis: col.ellipsis,
        align: col.align,
        width: col.width,
        render: col.render as ProColumns<T>['render'],
      };
      if (col.sorter) proCol.sorter = true;
      return proCol;
    }),
    [columns],
  );

  const request = async (params: Record<string, unknown>) => {
    const { current, pageSize, ...rest } = params as Record<string, unknown>;
    const requestParams: PageParams & SearchParams = {
      pageIndex: (current as number) ?? 1,
      pageSize: (pageSize as number) ?? defaultPageSize,
    };
    Object.entries(rest).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        requestParams[key] = value;
      }
    });
    try {
      const res = await fetchData(requestParams);
      return { data: res.data?.list ?? [], success: true, total: res.data?.total ?? 0 };
    } catch {
      return { data: [], success: false, total: 0 };
    }
  };

  return (
    <ProTable<T>
      actionRef={actionRef}
      rowKey={rowKey as string}
      columns={proColumns}
      search={columns.some(c => c.search) ? { labelWidth: 'auto' } : false}
      request={request}
      pagination={{ defaultPageSize, showSizeChanger: true }}
      toolBarRender={toolbarActions ? () => [toolbarActions] : undefined}
    />
  );
});

export default LegacyProTableInner;
