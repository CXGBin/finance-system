import React from 'react';
import type { RouteObject } from 'react-router-dom';

const routes: RouteObject[] = [
  {
    path: '/login',
    lazy: () => import('@/modules/login'),
  },
  {
    path: '/',
    lazy: () => import('@/components/Layout'),
    children: [
      {
        index: true,
        lazy: () => import('@/modules/dashboard'),
      },
      // 系统管理
      { path: 'system/user', lazy: () => import('@/modules/system/user') },
      { path: 'system/role', lazy: () => import('@/modules/system/role') },
      { path: 'system/menu', lazy: () => import('@/modules/system/menu') },
      { path: 'system/dept', lazy: () => import('@/modules/system/dept') },
      { path: 'system/post', lazy: () => import('@/modules/system/post') },
      { path: 'system/dict', lazy: () => import('@/modules/system/dict') },
      { path: 'system/log', lazy: () => import('@/modules/system/log') },
      { path: 'system/module', lazy: () => import('@/modules/system/module') },
      { path: 'system/config', lazy: () => import('@/modules/system/config') },
      // 账务管理
      { path: 'account/subject', lazy: () => import('@/modules/account/subject') },
      { path: 'account/balance', lazy: () => import('@/modules/account/balance') },
      { path: 'account/voucher', lazy: () => import('@/modules/account/voucher/list') },
      { path: 'account/voucher/add', lazy: () => import('@/modules/account/voucher/add') },
      { path: 'account/voucher/:id', lazy: () => import('@/modules/account/voucher/detail') },
      { path: 'account/ledger/general', lazy: () => import('@/modules/account/ledger/general') },
      { path: 'account/ledger/detail', lazy: () => import('@/modules/account/ledger/detail') },
      { path: 'account/ledger/journal', lazy: () => import('@/modules/account/ledger/journal') },
      { path: 'account/period', lazy: () => import('@/modules/account/period') },
      // 报表中心
      { path: 'report/balance-sheet', lazy: () => import('@/modules/report/balance-sheet') },
      { path: 'report/income-statement', lazy: () => import('@/modules/report/income-statement') },
      { path: 'report/cash-flow', lazy: () => import('@/modules/report/cash-flow') },
      { path: 'report/subject-balance', lazy: () => import('@/modules/report/subject-balance') },
      { path: 'report/custom', lazy: () => import('@/modules/report/custom') },
      // 预算管理
      { path: 'budget/setting', lazy: () => import('@/modules/budget/setting') },
      { path: 'budget/plan', lazy: () => import('@/modules/budget/plan') },
      { path: 'budget/execution', lazy: () => import('@/modules/budget/execution') },
      { path: 'budget/alert', lazy: () => import('@/modules/budget/alert') },
      { path: 'budget/analysis', lazy: () => import('@/modules/budget/analysis') },
      { path: 'budget/adjust', lazy: () => import('@/modules/budget/adjust') },
      // 审批流程
      { path: 'approval/pending', lazy: () => import('@/modules/approval/pending') },
      { path: 'approval/done', lazy: () => import('@/modules/approval/done') },
      { path: 'approval/my', lazy: () => import('@/modules/approval/my') },
      { path: 'approval/:id', lazy: () => import('@/modules/approval/detail') },
      { path: 'approval/template', lazy: () => import('@/modules/approval/template') },
      // 资产管理
      { path: 'asset/category', lazy: () => import('@/modules/asset/category') },
      { path: 'asset/card', lazy: () => import('@/modules/asset/card') },
      { path: 'asset/card/:id', lazy: () => import('@/modules/asset/card-detail') },
      { path: 'asset/depreciation', lazy: () => import('@/modules/asset/depreciation') },
      { path: 'asset/change', lazy: () => import('@/modules/asset/change') },
      { path: 'asset/inventory', lazy: () => import('@/modules/asset/inventory') },
      { path: 'asset/report', lazy: () => import('@/modules/asset/report') },
      // 费用管理
      { path: 'expense/type', lazy: () => import('@/modules/expense/type') },
      { path: 'expense/claim', lazy: () => import('@/modules/expense/claim/list') },
      { path: 'expense/claim/add', lazy: () => import('@/modules/expense/claim/add') },
      { path: 'expense/claim/:id', lazy: () => import('@/modules/expense/claim/detail') },
      { path: 'expense/payment', lazy: () => import('@/modules/expense/payment') },
      { path: 'expense/allocate', lazy: () => import('@/modules/expense/allocate') },
      { path: 'expense/statistics', lazy: () => import('@/modules/expense/statistics') },
      // 税务管理
      { path: 'tax/type', lazy: () => import('@/modules/tax/type') },
      { path: 'tax/declaration', lazy: () => import('@/modules/tax/declaration') },
      { path: 'tax/invoice', lazy: () => import('@/modules/tax/invoice') },
      { path: 'tax/report', lazy: () => import('@/modules/tax/report') },
      { path: 'tax/calendar', lazy: () => import('@/modules/tax/calendar') },
    ],
  },
  {
    path: '*',
    element: <div style={{ textAlign: 'center', padding: 100 }}>404 - 页面不存在</div>,
  },
];

export default routes;
