﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Tono
{
    public class CompressionHuffmanCodingHexNumber : CompressionHuffmanCodingPrepareTable
    {
        private static readonly Dictionary<BitList, byte> mytable = ReadTableBinary(new BitList(
            new byte[] { 0x00, 0x01, 0x00, 0xbb, 0x5f, 0x05, 0xf4, 0xde, 0x56, 0xa0, 0xf7, 0x76, 0x03, 0xbd, 0xb7, 0x27, 0xe8, 0xbd, 0x43, 0x41, 0xef, 0x1d, 0x0d, 0x7a, 0xef, 0x74, 0xd0, 0x7b, 0x67, 0x84, 0xde, 0xbb, 0x24, 0xf4, 0xde, 0x55, 0xa1, 0xf7, 0x6e, 0x0b, 0xbd, 0x77, 0x67, 0xe8, 0xbd, 0x67, 0x43, 0xef, 0x43, 0x1d, 0x7a, 0xef, 0xf5, 0xd0, 0x7b, 0x6f, 0x88, 0xde, 0xfb, 0x44, 0xf4, 0xde, 0x57, 0xa2, 0xf7, 0x7e, 0x13, 0xbd, 0xf7, 0xa7, 0xe8, 0x7d, 0x40, 0x45, 0xef, 0x03, 0x2d, 0x7a, 0x1f, 0x74, 0xd1, 0xfb, 0x60, 0x8c, 0xde, 0x87, 0x64, 0xf4, 0xde, 0x46, 0xa3, 0xf7, 0x1e, 0x1b, 0xbd, 0xb7, 0xe6, 0xe8, 0xbd, 0x4e, 0x47, 0xef, 0x15, 0x3d, 0x7a, 0xaf, 0xf4, 0xd1, 0x7b, 0x65, 0x90, 0x15, 0x43, 0x7a, 0xaf, 0x22, 0xd2, 0x7b, 0xd5, 0x91, 0xde, 0xab, 0x91, 0xf4, 0x5e, 0xbd, 0xa4, 0xf7, 0x1a, 0x26, 0xbd, 0xd7, 0x3c, 0xe9, 0xbd, 0x16, 0xca, 0xfa, 0x29, 0xeb, 0xa8, 0xb4, 0x6e, 0x65, 0x6d, 0x96, 0xde, 0x6b, 0xb7, 0xac, 0xe5, 0xb2, 0xe6, 0xcb, 0x1a, 0x30, 0x2b, 0xc7, 0xac, 0x24, 0xb3, 0xc2, 0xcc, 0xf2, 0x34, 0x4b, 0xd4, 0x2c, 0x6e, 0xb3, 0xe8, 0xcd, 0x6a, 0x38, 0xcb, 0xe4, 0x2c, 0xa8, 0xd3, 0x7b, 0xdd, 0x9d, 0xde, 0x5b, 0xf2, 0xf4, 0x5e, 0xaf, 0xa7, 0xf7, 0xfa, 0x3e, 0xbd, 0x37, 0xf8, 0xe9, 0xbd, 0x21, 0x50, 0xef, 0x8d, 0x82, 0x96, 0x14, 0x5a, 0x6a, 0x68, 0x69, 0xa2, 0x65, 0x8b, 0x56, 0x35, 0x5a, 0xee, 0xa8, 0xf7, 0xc6, 0x48, 0xbd, 0x37, 0x49, 0xea, 0xbd, 0xa9, 0x52, 0xef, 0xcd, 0x96, 0x7a, 0x6f, 0xce, 0xd4, 0x7b, 0x8b, 0xa6, 0xde, 0x5b, 0x39, 0xf5, 0x3e, 0xec, 0xa9, 0xf7, 0xe1, 0x50, 0xbd, 0x8f, 0x88, 0xea, 0x7d, 0xa9, 0x54, 0xef, 0xcb, 0xa6, 0x7a, 0x5f, 0x4e, 0xd5, 0xfb, 0x8a, 0xaa, 0xde, 0x57, 0x5a, 0xf5, 0xbe, 0xea, 0xaa, 0xf7, 0xd5, 0x58, 0xbd, 0xaf, 0xc9, 0xea, 0x7d, 0xad, 0x56, 0xef, 0xeb, 0xb6, 0xd6, 0xe3, 0xea, 0x7d, 0x7d, 0xd7, 0xba, 0x5e, 0xbd, 0x6f, 0xf8, 0xea, 0x7d, 0x23, 0x58, 0xef, 0x9b, 0xc2, 0x16, 0x13, 0x5b, 0x60, 0x6c, 0x11, 0xb2, 0x85, 0xcb, 0x16, 0x32, 0x5b, 0xe5, 0xac, 0xf7, 0xcd, 0x68, 0xbd, 0x6f, 0x49, 0xeb, 0x7d, 0xab, 0x5a, 0xef, 0xdb, 0xd6, 0x7a, 0xdf, 0xce, 0xd6, 0xfb, 0x8e, 0xb6, 0xde, 0x77, 0xba, 0xf5, 0xbe, 0xeb, 0xad, 0xf7, 0xdd, 0x70, 0xbd, 0xef, 0x89, 0xeb, 0x7d, 0xaf, 0x5c, 0xef, 0x4b, 0xe6, 0x7a, 0x5f, 0x4c, 0xd7, 0xfb, 0xa2, 0xba, 0xde, 0xa7, 0xd8, 0xf5, 0x3e, 0xf2, 0xae, 0xf7, 0x51, 0x78, 0xbd, 0x8f, 0xce, 0xeb, 0x7d, 0x8c, 0x5e, 0xef, 0x63, 0xf7, 0x7a, 0x1f, 0xc7, 0xd7, 0xfb, 0xf8, 0xbe, 0xde, 0x27, 0xf8, 0xf5, 0x3e, 0xf1, 0xaf, 0xf7, 0xf2, 0x80, 0xbd, 0x4f, 0x0e, 0xec, 0x7d, 0xaa, 0x60, 0xef, 0x0b, 0x07, 0x7b, 0x9f, 0x46, 0xd8, 0xfb, 0xf4, 0xc2, 0xde, 0x67, 0x18, 0xf6, 0x3e, 0xf3, 0xb0, 0xf7, 0x59, 0x88, 0xbd, 0xcf, 0x4e, 0xec, 0x7d, 0x8e, 0x62, 0xef, 0x73, 0x17, 0x7b, 0x9f, 0xc7, 0xd8, 0xfb, 0xfc, 0xc6, 0xde, 0x17, 0x38, 0xf6, 0x5e, 0xe1, 0xb1, 0xf7, 0x7d, 0x90, 0xbd, 0x97, 0x8b, 0xec, 0x3d, 0x8f, 0xe4, 0xee, 0xdf, 0x93, 0xbb, 0x7f, 0x51, 0xee, 0xfe, 0x59, 0xb9, 0xfb, 0x87, 0xe5, 0xee, 0xef, 0x97, 0xbb, 0xbf, 0x61, 0xee, 0xfe, 0x9a, 0xb9, 0xfb, 0x8f, 0xe6, 0xee, 0x2f, 0x9b, 0xbb, 0x3f, 0x71, 0xee, 0xfe, 0xd8, 0xb9, 0xfb, 0x83, 0xe7, 0xee, 0xf7, 0x9f, 0xbb, 0xdf, 0x81, 0xee, 0x7e, 0x1b, 0xba, 0xfb, 0x8d, 0xe8, 0xee, 0xcf, 0xa3, 0xbb, 0xff, 0x92, 0xee, 0xfe, 0x57, 0xba, 0xfb, 0xbf, 0x69, 0xef, 0x11, 0x4e, 0x7b, 0x0f, 0x8f, 0xda, 0x7b, 0xb8, 0xd4, 0xde, 0xc3, 0xaa, 0xf6, 0x1e, 0x66, 0xb5, 0xf7, 0xd0, 0xac, 0xbd, 0x87, 0x6a, 0xed, 0x3d, 0xa4, 0x6b, 0xef, 0x21, 0x5e, 0x7b, 0x0f, 0x0e, 0xdb, 0x7b, 0xb0, 0xd8, 0xde, 0x83, 0xca, 0xf6, 0x1e, 0x64, 0xb6, 0xf7, 0xc0, 0xb4, 0xbd, 0x07, 0xaa, 0xed, 0x3d, 0xa0, 0x6d, 0xef, 0x01, 0x6e, 0x77, 0xbf, 0xc6, 0xdd, 0xfd, 0x20, 0x77, 0xf7, 0x43, 0xdd, 0xdd, 0x8f, 0x76, 0x77, 0x3f, 0xe6, 0xdd, 0xfd, 0xa4, 0x77, 0xf7, 0x53, 0xdf, 0xdd, 0xcf, 0x7e, 0x77, 0x3f, 0x07, 0xde, 0xfd, 0x22, 0x78, 0xf7, 0x4b, 0xe1, 0xde, 0x23, 0x0e, 0xf7, 0x1e, 0x89, 0xb8, 0xf7, 0x0c, 0xc5, 0xbd, 0x47, 0x31, 0xee, 0x3d, 0xf3, 0x71, 0xef, 0x59, 0x90, 0x7b, 0xcf, 0x9a, 0xdc, 0x7b, 0x36, 0xe5, 0xde, 0xb3, 0x2f, 0xf7, 0x9e, 0x83, 0xb9, 0xf7, 0x9c, 0xcd, 0xbd, 0xe7, 0x72, 0xee, 0x3d, 0xf7, 0x73, 0xef, 0x91, 0xa1, 0x7b, 0xcf, 0x1b, 0xdd, 0x7b, 0x3e, 0xe9, 0xde, 0x0b, 0x4c, 0xf7, 0x5e, 0x96, 0xba, 0xf7, 0x82, 0xd5, 0xbd, 0x17, 0xb2, 0xee, 0xbd, 0xf0, 0x75, 0xef, 0x45, 0xb0, 0x7b, 0x2f, 0x9a, 0xdd, 0x7b, 0x31, 0xed, 0xde, 0x8b, 0x6f, 0xf7, 0x5e, 0x82, 0xbb, 0xf7, 0x92, 0xdd, 0xbd, 0x97, 0xf2, 0xee, 0xbd, 0xf4, 0x77, 0xef, 0x65, 0xc0, 0x7b, 0xcf, 0x14, 0xde, 0x7b, 0x7e, 0xf1, 0xde, 0x33, 0x8e, 0xf7, 0x9e, 0x90, 0xbc, 0xf7, 0xa8, 0xe5, 0xbd, 0x47, 0x33, 0xef, 0x3d, 0xfa, 0x79, 0xef, 0x31, 0xd0, 0x7b, 0x8f, 0x99, 0xde, 0x7b, 0x2c, 0xf5, 0xde, 0x63, 0xaf, 0xf7, 0x1e, 0x87, 0xbd, 0xf7, 0xb8, 0xed, 0xbd, 0xc7, 0x73, 0xef, 0x3d, 0xfe, 0x7b, 0xef, 0x09, 0xe0, 0x7b, 0x4f, 0x14, 0xdf, 0x7b, 0x7a, 0xf9, 0xde, 0x13, 0xcf, 0xf7, 0x9e, 0x84, 0xbe, 0xf7, 0xa4, 0xf5, 0xbd, 0x27, 0xb3, 0xef, 0x3d, 0xf9, 0x7d, 0xef, 0x29, 0xf0, 0x7b, 0x4f, 0x99, 0xdf, 0x7b, 0x2a, 0xfd, 0xde, 0x53, 0xef, 0xf7, 0x9e, 0x86, 0xbf, 0xf7, 0xb4, 0xfd, 0xbd, 0xa7, 0xf3, 0xef, 0x7d, 0xd2, 0x7f, 0xef, 0xfb, 0x01, }
        ), out var _);
        protected override Dictionary<BitList, byte> Table => mytable;
        protected override ushort TableID => 0x8005;
    }

    public class CompressionHuffmanCodingNumber : CompressionHuffmanCodingPrepareTable
    {
        private static readonly Dictionary<BitList, byte> mytable = ReadTableBinary(new BitList(
            new byte[] { 0x00, 0x01, 0x00, 0xf9, 0x6f, 0x03, 0xf6, 0x0f, 0x10, 0xd8, 0x3f, 0x70, 0x60, 0xff, 0x20, 0x82, 0xfd, 0x83, 0x0b, 0xf6, 0x0f, 0x31, 0xd8, 0x3f, 0xf4, 0x60, 0xff, 0x30, 0x84, 0xfd, 0xc3, 0x13, 0xf6, 0x8f, 0x50, 0xd8, 0x3f, 0x72, 0x61, 0xff, 0x28, 0x86, 0xfd, 0xa3, 0x1b, 0xf6, 0x8f, 0x75, 0xd8, 0x3f, 0xeb, 0x61, 0xff, 0x38, 0x88, 0xfd, 0xe3, 0x23, 0xf6, 0x4f, 0x90, 0xd8, 0x3f, 0x71, 0x62, 0xff, 0x24, 0x8a, 0xfd, 0x93, 0x2b, 0xf6, 0x4f, 0xb1, 0xd8, 0x3f, 0xf5, 0x62, 0xff, 0x34, 0x8c, 0xfd, 0xd3, 0x33, 0xf6, 0xcf, 0xd0, 0xd8, 0x3f, 0x73, 0x63, 0x7f, 0x7f, 0x8e, 0xfd, 0xfd, 0x3a, 0xf6, 0xf7, 0xf5, 0xd8, 0xdf, 0xe7, 0x63, 0x7f, 0x63, 0x90, 0xb4, 0x21, 0xfb, 0x9b, 0x88, 0xec, 0x6f, 0x3a, 0xb2, 0xbf, 0x19, 0xc9, 0xfe, 0xe6, 0x25, 0xfb, 0x5b, 0x98, 0xec, 0x6f, 0x79, 0xb2, 0xbf, 0x15, 0xca, 0xe2, 0x29, 0x8b, 0xa9, 0x2c, 0xba, 0xb2, 0x30, 0xcb, 0xfe, 0xd6, 0x2d, 0x0b, 0xba, 0x2c, 0xf0, 0xb2, 0x1e, 0x4c, 0xea, 0x98, 0x34, 0x32, 0x29, 0x67, 0x52, 0xd0, 0x24, 0xaf, 0x49, 0x66, 0x93, 0xf4, 0x26, 0x1d, 0x4e, 0xaa, 0x9c, 0x24, 0x3a, 0xfb, 0xdb, 0xec, 0xec, 0x6f, 0xcb, 0xb3, 0xbf, 0x5d, 0xcf, 0xfe, 0xf6, 0x3e, 0xfb, 0x3b, 0xfc, 0xec, 0xef, 0x08, 0xb4, 0xbf, 0x53, 0xd0, 0xfe, 0xce, 0x42, 0xfb, 0xbb, 0x0c, 0xed, 0xef, 0x4a, 0xb4, 0xbf, 0x5b, 0xd1, 0xfe, 0xee, 0x46, 0xfb, 0x7b, 0x1c, 0xed, 0xef, 0x89, 0xb4, 0xbf, 0x57, 0xd2, 0xfe, 0xde, 0x4a, 0xfb, 0xc7, 0x2c, 0xed, 0x9f, 0xcd, 0xb4, 0x7f, 0x4e, 0xd3, 0xfe, 0xb9, 0x4e, 0xfb, 0xf7, 0x3c, 0xed, 0xdf, 0x0b, 0xb5, 0x7f, 0x5f, 0xd4, 0xfe, 0xfd, 0x52, 0xfb, 0x0f, 0x4c, 0xed, 0x3f, 0x48, 0xb5, 0xff, 0x50, 0xd5, 0xfe, 0xc3, 0x56, 0xfb, 0x8f, 0x5c, 0xed, 0x3f, 0x8a, 0xb5, 0xff, 0x58, 0xd6, 0xfe, 0xe3, 0x5a, 0xfb, 0x4f, 0x6e, 0x2d, 0xc4, 0xb5, 0xff, 0x5a, 0xd7, 0x22, 0x5e, 0xfb, 0x4f, 0x7d, 0xed, 0x3f, 0x0d, 0xb6, 0xff, 0x4c, 0xd8, 0xfe, 0xb3, 0x62, 0xfb, 0xcf, 0x8d, 0xed, 0x3f, 0x4f, 0xb6, 0xff, 0x42, 0xd9, 0xfe, 0x8b, 0x66, 0xfb, 0x2f, 0x9d, 0xed, 0xbf, 0x8c, 0xb6, 0xff, 0x4a, 0xda, 0xfe, 0xab, 0x6a, 0xfb, 0x77, 0xaf, 0xed, 0x3f, 0xc1, 0xb6, 0x7f, 0x57, 0xdb, 0xfe, 0x55, 0x6e, 0xfb, 0xe7, 0xbd, 0xed, 0x9f, 0x0f, 0xb7, 0x7f, 0x41, 0xdc, 0xfe, 0x85, 0x72, 0xfb, 0x17, 0xcd, 0xed, 0x5f, 0x4c, 0xb7, 0x7f, 0x49, 0xdd, 0xfe, 0xa5, 0x76, 0xfb, 0x97, 0xdd, 0xed, 0x5f, 0x8e, 0xb7, 0x7f, 0x45, 0xde, 0xfe, 0x95, 0x7a, 0xfb, 0x57, 0xef, 0xed, 0xdf, 0xc1, 0xb7, 0x7f, 0x4d, 0xdf, 0xfe, 0xb5, 0x7e, 0xfb, 0xd7, 0xfd, 0xed, 0x6f, 0x08, 0xb8, 0x7f, 0x43, 0xe0, 0xfe, 0x8d, 0x82, 0xfb, 0x37, 0x0d, 0xee, 0xdf, 0x4c, 0xb8, 0x7f, 0x4b, 0xe1, 0xfe, 0xad, 0x86, 0xfb, 0xb7, 0x1d, 0xee, 0xdf, 0x8e, 0xb8, 0xbf, 0x51, 0xe2, 0xfe, 0x9d, 0x8a, 0xfb, 0x1b, 0x2c, 0xee, 0x2f, 0xcd, 0xb8, 0x3f, 0x5c, 0xe3, 0xfe, 0xb0, 0x8e, 0xfb, 0xc3, 0x3c, 0xee, 0x0f, 0x0d, 0xb9, 0x3f, 0x54, 0xe4, 0xfe, 0x90, 0x92, 0xfb, 0x43, 0x4c, 0xee, 0x0f, 0x4e, 0xb9, 0x3f, 0x58, 0xe5, 0xfe, 0xa0, 0x96, 0xfb, 0x83, 0x5c, 0xee, 0x0f, 0x8c, 0xb9, 0x3f, 0x50, 0xe6, 0xfe, 0x80, 0x9a, 0xfb, 0x03, 0x6c, 0xee, 0x0f, 0xcf, 0xb9, 0x3f, 0x42, 0xe7, 0xfe, 0x88, 0x9e, 0xfb, 0x23, 0x7d, 0xee, 0x4f, 0x04, 0xba, 0x3f, 0x61, 0xe8, 0xfe, 0x04, 0xa2, 0xfb, 0xe3, 0x8f, 0xee, 0x8f, 0x47, 0xba, 0x3f, 0x6e, 0xe9, 0xfe, 0x38, 0xa6, 0xfb, 0x13, 0x9f, 0xee, 0x8f, 0x8d, 0xba, 0x3f, 0x66, 0xea, 0xfe, 0x18, 0xaa, 0xfb, 0xa3, 0xaf, 0xee, 0x8f, 0xc6, 0xba, 0x3f, 0x6a, 0xeb, 0xfe, 0x28, 0xae, 0xfb, 0x23, 0xbf, 0xe6, 0xff, 0x87, 0xcd, 0xff, 0x17, 0x9b, 0xff, 0x57, 0x36, 0xff, 0xcd, 0x6c, 0xfe, 0x1f, 0xda, 0xfc, 0x77, 0xb5, 0xf9, 0xef, 0x6d, 0xf3, 0x3f, 0xdc, 0xe6, 0x7f, 0xc4, 0xcd, 0xff, 0x94, 0x9b, 0xff, 0x59, 0x37, 0xff, 0xcb, 0x6e, 0xfe, 0x57, 0xde, 0xfc, 0x6f, 0xbd, 0xf9, 0xdf, 0x7d, 0xf3, 0x7f, 0xfc, 0xe6, 0xff, 0x04, 0xce, 0xff, 0x15, 0x9c, 0xff, 0x5b, 0x78, 0x7f, 0xac, 0xe1, 0xfd, 0x49, 0x88, 0xf7, 0x17, 0x2a, 0xde, 0x9f, 0xcc, 0x78, 0x7f, 0xd1, 0xe3, 0xfd, 0xc5, 0x90, 0xf7, 0x17, 0x4f, 0xde, 0x5f, 0x42, 0x79, 0x7f, 0xc9, 0xe5, 0xfd, 0xa5, 0x98, 0xf7, 0x27, 0x6d, 0xde, 0x5f, 0xc6, 0x79, 0x7f, 0xd9, 0xe7, 0xfd, 0xe5, 0xa0, 0xf7, 0x97, 0x8f, 0xde, 0x5f, 0x41, 0x7a, 0x7f, 0xa5, 0xe9, 0xfd, 0xf5, 0xa9, 0xf7, 0x57, 0xae, 0xde, 0x5f, 0xc5, 0x7a, 0x7f, 0xd5, 0xeb, 0xfd, 0xd5, 0xb0, 0xf7, 0x57, 0xcf, 0xde, 0x5f, 0x43, 0x7b, 0x7f, 0xcd, 0xed, 0xfd, 0xb5, 0xb8, 0xf7, 0xd7, 0xee, 0xde, 0x5f, 0xc7, 0x7b, 0x7f, 0xdd, 0xef, 0xfd, 0xf5, 0xc0, 0xf7, 0x17, 0x09, 0xdf, 0x5f, 0x51, 0x7c, 0x7f, 0xe1, 0xf1, 0xfd, 0x99, 0xc9, 0xf7, 0x27, 0x2f, 0xdf, 0x9f, 0xc2, 0x7c, 0x7f, 0xca, 0xf3, 0xfd, 0xa9, 0xd0, 0xf7, 0xa7, 0x4e, 0xdf, 0x9f, 0x46, 0x7d, 0x7f, 0xda, 0xf5, 0xfd, 0xe9, 0xd8, 0xf7, 0xa7, 0x6f, 0xdf, 0x9f, 0xc1, 0x7d, 0x7f, 0xc6, 0xf7, 0xfd, 0x99, 0xe0, 0xf7, 0x67, 0x89, 0xdf, 0x5f, 0x50, 0x7e, 0x7f, 0xd6, 0xf9, 0xfd, 0xd9, 0xe8, 0xf7, 0x67, 0xaf, 0xdf, 0x9f, 0xc3, 0x7e, 0x7f, 0xce, 0xfb, 0xfd, 0xb9, 0xf0, 0xf7, 0xe7, 0xce, 0xdf, 0x9f, 0x47, 0x7f, 0x7f, 0xde, 0xfd, 0xfd, 0xf9, 0xf8, 0xf7, 0xe7, 0xef, 0xdf, 0x5f, 0xc0, 0x7f, 0xff, 0xfa, 0xff, 0xfd, 0xd7, 0x01, }
        ), out var _);
        protected override Dictionary<BitList, byte> Table => mytable;
        protected override ushort TableID => 0x8004;
    }

    public class CompressionHuffmanCodingBase64 : CompressionHuffmanCodingPrepareTable
    {
        private static readonly Dictionary<BitList, byte> mytable = ReadTableBinary(new BitList(
            new byte[] { 0x00, 0x01, 0x00, 0x42, 0xd8, 0x53, 0x01, 0x46, 0xd8, 0xa1, 0x04, 0x8c, 0xb0, 0x43, 0x0f, 0x18, 0x61, 0x87, 0x21, 0x30, 0xc2, 0x0e, 0x5b, 0x60, 0x84, 0x1d, 0xce, 0xc0, 0x08, 0x3b, 0xfc, 0x81, 0x11, 0x76, 0x04, 0x04, 0x23, 0xec, 0x88, 0x09, 0x46, 0xd8, 0x91, 0x14, 0x8c, 0xb0, 0x23, 0x2f, 0x18, 0x61, 0x47, 0x61, 0x30, 0xc2, 0x8e, 0xda, 0x60, 0x84, 0x1d, 0xcd, 0xc1, 0x08, 0x3b, 0xfa, 0x83, 0x11, 0x76, 0x0c, 0x08, 0x23, 0xec, 0x98, 0x11, 0x46, 0xd8, 0xb1, 0x24, 0x8c, 0xb0, 0x43, 0x4e, 0x18, 0x61, 0x87, 0xa0, 0x30, 0xc2, 0x0e, 0x5e, 0x21, 0x84, 0xfd, 0x6b, 0x21, 0x84, 0xfd, 0x76, 0x21, 0x84, 0xfd, 0x8e, 0x21, 0x84, 0xfd, 0x91, 0x21, 0x84, 0xfd, 0xa9, 0x21, 0x84, 0xfd, 0xb5, 0x21, 0x84, 0xfd, 0xcd, 0x21, 0x84, 0xfd, 0xd3, 0x21, 0x84, 0xfd, 0xe7, 0x61, 0x84, 0x1d, 0xec, 0x43, 0x08, 0xfb, 0x1f, 0x24, 0x62, 0x85, 0x44, 0x12, 0x91, 0x48, 0x3c, 0x12, 0x89, 0x48, 0x22, 0x45, 0x49, 0xa4, 0x33, 0x89, 0x1c, 0x27, 0x91, 0x01, 0x65, 0x62, 0x4b, 0x89, 0xdc, 0x2a, 0x91, 0x6b, 0xe5, 0x11, 0x58, 0x22, 0x67, 0x8b, 0x11, 0x76, 0x00, 0x97, 0xc8, 0xfc, 0xf2, 0x70, 0x30, 0x0f, 0x1e, 0xf3, 0x10, 0x32, 0x0f, 0x35, 0xf3, 0xd0, 0x34, 0x0f, 0x53, 0xf3, 0x60, 0x36, 0x0f, 0x79, 0xf3, 0x20, 0x38, 0x6d, 0xcb, 0x69, 0x99, 0x4e, 0x64, 0xdb, 0x89, 0xec, 0x3c, 0x91, 0xb5, 0xe7, 0xe1, 0x7d, 0x22, 0xcb, 0x4f, 0x64, 0x04, 0x8a, 0x4c, 0x41, 0x0f, 0x2a, 0xd4, 0xd2, 0xa1, 0x96, 0x10, 0xb5, 0xb8, 0xa8, 0x45, 0x46, 0x2d, 0x3c, 0x6a, 0x01, 0x52, 0xf3, 0x93, 0x9a, 0xa7, 0xd4, 0xdc, 0xa5, 0xe6, 0x30, 0x35, 0xbb, 0xa9, 0x59, 0x4e, 0xcd, 0x7c, 0x6a, 0x06, 0x54, 0xd3, 0xa3, 0x9a, 0x26, 0xd5, 0xd4, 0xa9, 0xa6, 0x50, 0x35, 0xb9, 0xaa, 0x49, 0x56, 0x8d, 0xbf, 0x6a, 0x22, 0x56, 0x13, 0xb2, 0x5a, 0xae, 0xd5, 0x8a, 0xad, 0x48, 0xca, 0x15, 0x69, 0xbb, 0x22, 0xbd, 0x97, 0x11, 0x76, 0xc0, 0xaf, 0x48, 0x05, 0x16, 0xa9, 0xc3, 0x5a, 0x29, 0xd6, 0xaa, 0xb1, 0x07, 0x22, 0x7b, 0xc0, 0xb2, 0x07, 0x30, 0x6b, 0xff, 0x59, 0xfb, 0xd0, 0xda, 0x9b, 0xd6, 0x1e, 0xb5, 0x76, 0xaf, 0xb5, 0x8b, 0xad, 0x9d, 0x6d, 0xed, 0x70, 0x7b, 0xe0, 0xb7, 0xb6, 0xc3, 0xb5, 0x35, 0xae, 0x2d, 0x72, 0x6d, 0x9e, 0x6b, 0x13, 0x5d, 0x1b, 0xeb, 0xda, 0x60, 0xd7, 0xfa, 0xbb, 0xd6, 0xe1, 0xb5, 0x36, 0xaf, 0x35, 0x7a, 0xad, 0xde, 0x8b, 0x34, 0x7c, 0x91, 0xac, 0x2f, 0x92, 0xfb, 0x45, 0xca, 0x3f, 0x21, 0xec, 0x17, 0x40, 0x23, 0xec, 0xc0, 0x81, 0x46, 0xd8, 0x41, 0x04, 0x8d, 0xb0, 0x83, 0x0e, 0x1a, 0x61, 0xc7, 0x26, 0x34, 0xc2, 0x8e, 0x5b, 0x68, 0x84, 0x5d, 0xc4, 0xd0, 0x08, 0x3b, 0xde, 0xa1, 0x11, 0x76, 0x66, 0x44, 0x23, 0xec, 0x2c, 0x89, 0x46, 0xd8, 0x59, 0x15, 0x8d, 0xb0, 0xb3, 0x2d, 0x1a, 0x61, 0x67, 0x67, 0x34, 0xc2, 0xce, 0xd1, 0x68, 0x84, 0x9d, 0xd3, 0xd1, 0x08, 0x3b, 0xd7, 0xa3, 0x11, 0x76, 0x6e, 0x48, 0x23, 0xec, 0x3c, 0x91, 0x46, 0xd8, 0x79, 0x25, 0x8d, 0xb0, 0xf3, 0x4d, 0x1a, 0x61, 0xe7, 0xa7, 0x34, 0xc2, 0x2e, 0x50, 0x69, 0x84, 0x5d, 0xd0, 0xd2, 0x08, 0xbb, 0xd0, 0xa5, 0x11, 0x76, 0x61, 0x4c, 0x23, 0xec, 0x4c, 0x99, 0x46, 0xd8, 0x19, 0x35, 0x8d, 0xb0, 0x33, 0x6c, 0x1a, 0x61, 0x27, 0xe3, 0x34, 0xc2, 0x8e, 0xdf, 0x69, 0x84, 0x9d, 0xc0, 0xd3, 0x08, 0x3b, 0xe1, 0xa7, 0x11, 0x76, 0x22, 0x50, 0x23, 0xec, 0xc4, 0xa1, 0x46, 0xd8, 0x49, 0x44, 0x8d, 0xb0, 0x93, 0x8e, 0x1a, 0x61, 0x27, 0x27, 0x35, 0xc2, 0x4e, 0x5f, 0x6a, 0x84, 0x9d, 0xc2, 0xd4, 0x08, 0x3b, 0xe5, 0xa9, 0x11, 0x76, 0x2a, 0x54, 0x23, 0xec, 0xd4, 0xa9, 0x46, 0xd8, 0x69, 0x54, 0x8d, 0xb0, 0xd3, 0xae, 0x1a, 0x61, 0xa7, 0x63, 0x15, 0xc2, 0x7e, 0x6d, 0x35, 0xc2, 0x8e, 0xe3, 0x2a, 0x84, 0xfd, 0xfc, 0x2a, 0x84, 0x7d, 0x06, 0x2b, 0x84, 0x5d, 0x1b, 0x2b, 0x84, 0x5d, 0x29, 0x2b, 0x84, 0x5d, 0x3d, 0x2b, 0x84, 0x5d, 0x45, 0x2b, 0x84, 0xdd, 0x58, 0x2b, 0x84, 0x5d, 0x61, 0x2b, 0x84, 0x5d, 0x7c, 0x2b, 0x84, 0x5d, 0x86, 0x2b, 0x84, 0x5d, 0x9a, 0x2b, 0x84, 0x5d, 0xae, 0x2b, 0x84, 0x5d, 0xb2, 0x2b, 0x84, 0x5d, 0xc3, 0x2b, 0x84, 0xdd, 0xd4, 0x2b, 0x84, 0x3d, 0xe1, 0x2b, 0x84, 0xdd, 0xf2, 0x2b, 0x84, 0x3d, 0x0e, 0x2c, 0x84, 0x3d, 0x16, 0x2c, 0x84, 0xdd, 0x2c, 0x2c, 0x84, 0x3d, 0x3a, 0x2c, 0x84, 0x3d, 0x42, 0x2c, 0x84, 0x3d, 0x5c, 0x2c, 0x84, 0x3d, 0x64, 0x2c, 0x84, 0x3d, 0x78, 0x2c, 0x84, 0x3d, 0x80, 0x2c, 0x84, 0xdd, 0x9f, 0x2c, 0x84, 0xdd, 0xa7, 0x2c, 0x84, 0xdd, 0xb5, 0x2c, 0x84, 0xdd, 0xc9, 0x2c, 0x84, 0xdd, 0xd1, 0x2c, 0x84, 0xdd, 0xee, 0x2c, 0x84, 0xdd, 0xfa, 0x2c, 0x84, 0xdd, 0x0d, 0x2d, 0x84, 0xdd, 0x16, 0x2d, 0x84, 0xdd, 0x2b, 0x2d, 0x84, 0x5d, 0x37, 0x2d, 0x84, 0x5d, 0x4f, 0x2d, 0x84, 0xdd, 0x50, 0x2d, 0x84, 0xdd, 0x63, 0x2d, 0x84, 0x3d, 0x79, 0x2d, 0x84, 0xbd, 0x87, 0x2d, 0x84, 0x3d, 0x9d, 0x2d, 0x84, 0x7d, 0xa8, 0x2d, 0x84, 0x7d, 0xb4, 0x2d, 0x84, 0x7d, 0xcc, 0x2d, 0x84, 0x7d, 0xd2, 0x2d, 0x84, 0x7d, 0xea, 0x2d, 0x84, 0x7d, 0xfe, 0x2d, 0x84, 0x7d, 0x01, 0x2e, 0x84, 0x7d, 0x19, 0x2e, 0x84, 0x7d, 0x25, 0x2e, 0x84, 0x7d, 0x3d, 0x2e, 0x84, 0x7d, 0x43, 0x2e, 0x84, 0x7d, 0x5b, 0x2e, 0x84, 0x7d, 0x67, 0x2e, 0x84, 0x7d, 0x7f, 0x2e, 0x84, 0xfd, 0x80, 0x2e, 0x84, 0xfd, 0x98, 0x2e, 0x84, 0x7d, 0xa0, 0x2e, 0x84, 0xfd, 0xb4, 0x2e, 0x84, 0xbd, 0xcf, 0x2e, 0x84, 0xbd, 0xdb, 0x2e, 0x84, 0x3d, 0xe3, 0x2e, 0x84, 0x3d, 0xfb, 0x2e, 0x84, 0x3d, 0x07, 0x2f, 0x84, 0x3d, 0x1f, 0x2f, 0x84, 0xbd, 0x20, 0x2f, 0x84, 0xbd, 0x38, 0x2f, 0x84, 0xbd, 0x44, 0x2f, 0x84, 0xbd, 0x5c, 0x2f, 0x84, 0xbd, 0x62, 0x2f, 0x84, 0xbd, 0x7a, 0x2f, 0x84, 0xbd, 0x86, 0x2f, 0x84, 0xbd, 0x9e, 0x2f, 0x84, 0xbd, 0xa1, 0x2f, 0x84, 0xbd, 0xb9, 0x2f, 0x84, 0xbd, 0xc5, 0x2f, 0x84, 0xbd, 0xdd, 0x2f, 0x84, 0xbd, 0xe3, 0x6f, 0x84, 0x1d, 0xe8, 0xdf, 0x08, 0xbb, 0x28, }
        ), out var _);
        protected override Dictionary<BitList, byte> Table => mytable;
        protected override ushort TableID => 0x8003;
    }
    
    public class CompressionHuffmanCodingJapanese : CompressionHuffmanCodingPrepareTable
    {
        private static readonly Dictionary<BitList, byte> mytable = ReadTableBinary(new BitList(
            new byte[] { 0x00, 0x01, 0x00, 0x52, 0xec, 0x3a, 0x1a, 0x20, 0xc5, 0xae, 0x7b, 0x02, 0x52, 0xec, 0xba, 0x3f, 0x20, 0xc5, 0xae, 0x07, 0x04, 0x52, 0xec, 0x7a, 0x58, 0x20, 0xc5, 0xae, 0x47, 0x06, 0x52, 0xec, 0x7a, 0x7c, 0x20, 0xc5, 0xae, 0x27, 0x08, 0x52, 0xec, 0x7a, 0x9a, 0x20, 0xc5, 0xae, 0xe7, 0x0a, 0x52, 0xec, 0xfa, 0xb0, 0x20, 0xc5, 0xae, 0x17, 0x0c, 0x52, 0xec, 0x7a, 0xd9, 0x20, 0xc5, 0xae, 0x57, 0x0e, 0x52, 0xec, 0x7a, 0xfd, 0x20, 0xc5, 0xae, 0x37, 0x10, 0x52, 0xec, 0x7a, 0x1b, 0x21, 0xc5, 0xae, 0x77, 0x12, 0x52, 0xec, 0x7a, 0x3f, 0x21, 0xc5, 0xae, 0xbb, 0x14, 0x52, 0xec, 0x7a, 0x56, 0x21, 0xc5, 0xae, 0x3b, 0x16, 0x52, 0xec, 0xba, 0x70, 0x21, 0xc5, 0xae, 0x13, 0x18, 0x52, 0xec, 0x3a, 0x99, 0x21, 0xc5, 0xae, 0x53, 0x1a, 0x52, 0xec, 0x3a, 0xbd, 0x21, 0xc5, 0xae, 0x33, 0x1c, 0x52, 0xec, 0x3a, 0xdb, 0x21, 0xc5, 0xae, 0x73, 0x1e, 0x52, 0xec, 0x3a, 0xff, 0x21, 0xc5, 0xae, 0x8b, 0x20, 0x95, 0x9d, 0x10, 0x29, 0x76, 0xdd, 0x12, 0x91, 0x62, 0xd7, 0xa5, 0x91, 0x8c, 0x33, 0x24, 0x52, 0xec, 0xba, 0x5c, 0x22, 0xc5, 0xae, 0x2b, 0x26, 0x52, 0xec, 0xba, 0x7a, 0x22, 0xc5, 0xae, 0x6b, 0x28, 0x52, 0xec, 0xba, 0x9e, 0xb2, 0x01, 0x8f, 0x8a, 0x14, 0xbb, 0x6e, 0xac, 0x48, 0xb1, 0xeb, 0x26, 0x8b, 0x14, 0xbb, 0x6e, 0xb7, 0x48, 0xb1, 0xeb, 0x93, 0xcb, 0xc6, 0x89, 0x2f, 0x56, 0xec, 0x1a, 0x1c, 0x26, 0x1a, 0xc6, 0x4c, 0x1c, 0x32, 0xd5, 0x80, 0x19, 0x29, 0x76, 0x7d, 0xa6, 0xb9, 0xc2, 0xa9, 0xc9, 0xec, 0x62, 0x93, 0x71, 0xf6, 0x66, 0x03, 0x4e, 0x9c, 0x8d, 0x13, 0x72, 0x2e, 0x1e, 0x75, 0xac, 0xd8, 0x35, 0xd4, 0x8e, 0x15, 0xbb, 0x86, 0xe6, 0xb1, 0x62, 0xd7, 0x30, 0x3d, 0x56, 0xec, 0x1a, 0xd6, 0xc7, 0x8a, 0x5d, 0xc3, 0xfd, 0x58, 0xb1, 0x6b, 0x78, 0x20, 0x2b, 0x76, 0x8d, 0x10, 0xb4, 0x01, 0xbf, 0x90, 0x15, 0xbb, 0x46, 0x1c, 0xaa, 0xc2, 0x13, 0x6d, 0x76, 0x55, 0x64, 0xc5, 0xae, 0x91, 0x8d, 0xac, 0xd8, 0x35, 0xce, 0x91, 0x15, 0xbb, 0x46, 0x41, 0xb2, 0x62, 0xd7, 0xa8, 0x49, 0x56, 0xec, 0x1a, 0x4d, 0x69, 0x03, 0xbe, 0x25, 0x2b, 0x76, 0x8d, 0xce, 0x64, 0xc5, 0xae, 0x31, 0x9a, 0xac, 0xd8, 0x35, 0xa6, 0xd3, 0x06, 0x5c, 0x4f, 0x56, 0xec, 0x1a, 0x0b, 0xaa, 0x56, 0x11, 0x65, 0xc5, 0xae, 0xb1, 0xa5, 0xac, 0xd8, 0x35, 0xe4, 0x54, 0xc6, 0x69, 0x2a, 0x2b, 0x76, 0x8d, 0x54, 0x65, 0xc5, 0xae, 0x21, 0xac, 0xa4, 0xd8, 0xf5, 0xe7, 0x4a, 0x8a, 0x5d, 0x5f, 0xb0, 0xa4, 0xd8, 0xf5, 0x35, 0x4b, 0x8a, 0x5d, 0xdf, 0xb4, 0xa4, 0xd8, 0xf5, 0x7d, 0x6b, 0x03, 0x0e, 0x2e, 0x29, 0x76, 0xfd, 0xe8, 0xda, 0x80, 0xd7, 0x4b, 0x8a, 0x5d, 0x3f, 0xbf, 0xa4, 0xd8, 0xf5, 0x0b, 0x4c, 0x8a, 0x5d, 0xbf, 0xc3, 0x26, 0x5d, 0xb1, 0x2b, 0xdc, 0xb1, 0x69, 0x90, 0x4c, 0x8a, 0x5d, 0x7f, 0xcb, 0x22, 0xbd, 0x99, 0x15, 0xbb, 0x06, 0x3b, 0xdb, 0x80, 0x1b, 0xed, 0xb2, 0x3b, 0x6d, 0x0a, 0x51, 0x93, 0x62, 0xd7, 0xbf, 0xb5, 0x8c, 0x53, 0x6c, 0x17, 0xcf, 0x6d, 0x19, 0x27, 0xdd, 0xaa, 0x81, 0x6f, 0x17, 0x4f, 0x70, 0x15, 0xb0, 0x38, 0x29, 0x76, 0xfd, 0x97, 0x8b, 0x9e, 0x73, 0x11, 0x82, 0xee, 0xe2, 0xbc, 0x2e, 0xd6, 0xda, 0x5d, 0x3c, 0xdc, 0x59, 0xb1, 0x6b, 0x00, 0x3c, 0x2b, 0x76, 0x0d, 0x98, 0x57, 0xab, 0xd4, 0xb3, 0x62, 0xd7, 0x40, 0x7b, 0x56, 0xec, 0x1a, 0x98, 0xcf, 0x8a, 0x5d, 0x83, 0xf4, 0x59, 0xb1, 0x6b, 0x50, 0x3f, 0x29, 0x76, 0x1d, 0xff, 0x93, 0x62, 0xd7, 0x31, 0xc0, 0xd5, 0x02, 0x4f, 0x04, 0x93, 0x0e, 0xae, 0x42, 0xf8, 0xb8, 0x85, 0x53, 0x98, 0x61, 0x34, 0x3a, 0x8c, 0xcc, 0x88, 0x0f, 0x91, 0xd8, 0x7a, 0xc5, 0x08, 0xbf, 0xd8, 0x2c, 0xc6, 0x87, 0x69, 0x8c, 0xa6, 0x8e, 0x13, 0xea, 0x63, 0xc4, 0x87, 0x9c, 0x74, 0x22, 0xa3, 0x87, 0x64, 0x14, 0x9d, 0x8c, 0x00, 0x94, 0x11, 0xae, 0x32, 0xb2, 0x59, 0xc6, 0x9a, 0xcb, 0x87, 0xc2, 0x9c, 0xc6, 0x32, 0xa3, 0x98, 0x66, 0x14, 0xdf, 0x9c, 0x68, 0x38, 0x9f, 0xb9, 0x73, 0x0a, 0xf5, 0x7c, 0xd8, 0xcf, 0xd6, 0x81, 0x4e, 0x40, 0xa1, 0xaf, 0x22, 0x1a, 0x99, 0x46, 0x27, 0x7b, 0xd2, 0x67, 0x2b, 0x7d, 0x7e, 0xd3, 0x27, 0x38, 0x7d, 0x34, 0xd4, 0x46, 0xa6, 0x3e, 0xa4, 0xea, 0xe3, 0xac, 0x3e, 0x16, 0x6b, 0x84, 0x6c, 0x8d, 0xde, 0xae, 0x0d, 0x7c, 0x6d, 0x3c, 0xec, 0xac, 0x1a, 0xfb, 0xec, 0xb2, 0xd3, 0xf8, 0x6c, 0x34, 0xa1, 0x9d, 0x68, 0x6b, 0x5f, 0xdd, 0x36, 0x8a, 0xdc, 0x4e, 0xbc, 0xb8, 0x11, 0x3d, 0xf7, 0xf9, 0x74, 0x23, 0xf4, 0x6e, 0xf4, 0xe2, 0x7d, 0xc0, 0xde, 0x88, 0xeb, 0x3b, 0xa1, 0xfc, 0xc6, 0x6a, 0x60, 0x29, 0x76, 0xad, 0x0e, 0x96, 0x62, 0xd7, 0x1a, 0x61, 0x29, 0x76, 0xad, 0x1d, 0x96, 0x62, 0xd7, 0x3a, 0x62, 0x29, 0x76, 0xad, 0x2f, 0x96, 0x62, 0xd7, 0x06, 0x63, 0x29, 0x76, 0x6d, 0x3c, 0x96, 0x62, 0xd7, 0x26, 0x64, 0x29, 0x76, 0x6d, 0x4e, 0x96, 0x62, 0xd7, 0x16, 0x65, 0x29, 0x76, 0x6d, 0x5d, 0x96, 0x62, 0xd7, 0x36, 0x66, 0x29, 0x76, 0x6d, 0x6f, 0x96, 0x62, 0xd7, 0x0e, 0x67, 0x29, 0x76, 0xed, 0x7c, 0x96, 0x62, 0xd7, 0x2e, 0x68, 0x29, 0x76, 0xed, 0x8e, 0x96, 0x62, 0xd7, 0x1e, 0x69, 0x29, 0x76, 0xed, 0x9d, 0x96, 0x62, 0xd7, 0x3e, 0x6a, 0x29, 0x76, 0xed, 0xaf, 0x96, 0x62, 0xd7, 0x01, 0x6b, 0x29, 0x76, 0x1d, 0xbc, 0x96, 0x62, 0xd7, 0x4a, 0x6c, 0x29, 0x76, 0x1d, 0xca, 0x96, 0x62, 0xd7, 0x61, 0x6d, 0x29, 0x76, 0xad, 0xda, 0x96, 0x62, 0xd7, 0x11, 0x6e, 0x29, 0x76, 0xad, 0xe8, 0x96, 0x62, 0xd7, 0x14, 0x6f, 0x29, 0x76, 0x4d, 0xfe, 0x96, 0x62, 0xd7, 0x24, 0x70, 0x29, 0x76, 0x4d, 0x0c, 0x97, 0x62, 0xd7, 0x78, 0x71, 0x29, 0x76, 0x4d, 0x18, 0x2f, 0x93, 0x3f, 0x4a, 0xf9, 0x92, 0xe6, 0x8d, 0x38, 0x5f, 0x0e, 0xfd, 0xc1, 0xd2, 0x1f, 0x43, 0x5d, 0x8a, 0x5d, 0x53, 0xd7, 0xa5, 0xd8, 0x35, 0x8d, 0x5d, 0x8a, 0x5d, 0xf3, 0xdb, 0xa5, 0xd8, 0x35, 0xc3, 0x5d, 0x8a, 0x5d, 0xcb, 0xde, 0x1f, 0x15, 0x5e, 0x8a, 0x5d, 0xd3, 0xe3, 0xa5, 0xd8, 0xb5, 0x54, 0x5e, 0x8a, 0x5d, 0x4b, 0xe6, 0xa5, 0xd8, 0xb5, 0x98, 0x5e, 0x8a, 0x5d, 0x8b, 0xea, 0xa5, 0xd8, 0xb5, 0xd0, 0x5e, 0x8a, 0x5d, 0xcb, 0xef, 0xa5, 0xd8, 0xb5, 0x00, 0x5f, 0x8a, 0x5d, 0xf3, 0xf2, 0xa5, 0xd8, 0x35, 0x57, 0x5f, 0x8a, 0x5d, 0x73, 0xf6, 0xa5, 0xd8, 0x35, 0x9b, 0x5f, 0x8a, 0x5d, 0xb3, 0xfa, 0xa5, 0xd8, 0x35, 0xd3, 0x5f, 0x8a, 0x5d, 0x1f, 0xff, 0xad, 0xd8, 0x35, 0x2e, }
        ), out var _);
        protected override Dictionary<BitList, byte> Table => mytable;
        protected override ushort TableID => 0x8002;
    }

    public class CompressionHuffmanCodingASCII : CompressionHuffmanCodingPrepareTable
    {
        private static readonly Dictionary<BitList, byte> mytable = ReadTableBinary(new BitList(
            new byte[] { 0x00, 0x01, 0x00, 0x4e, 0xd2, 0x1c, 0x0e, 0x90, 0x92, 0x66, 0x20, 0x01, 0x29, 0x69, 0x06, 0x1e, 0x90, 0x92, 0x66, 0x10, 0x02, 0x29, 0x69, 0x06, 0x2d, 0x90, 0x92, 0x66, 0x30, 0x03, 0x29, 0x69, 0x06, 0x3f, 0x90, 0x92, 0x66, 0x08, 0x04, 0x29, 0x69, 0x86, 0x4a, 0xb8, 0xd2, 0x50, 0x90, 0x92, 0x66, 0xa4, 0x05, 0x29, 0x69, 0x86, 0x66, 0x90, 0x92, 0x66, 0x98, 0x06, 0x29, 0x69, 0x86, 0x75, 0x90, 0x92, 0x66, 0xb8, 0x07, 0x29, 0x69, 0x86, 0x87, 0x90, 0x92, 0x66, 0x84, 0x08, 0x29, 0x69, 0x46, 0x94, 0x90, 0x92, 0x66, 0xc0, 0x09, 0x29, 0x69, 0x86, 0xa4, 0x90, 0x92, 0x66, 0x80, 0x0a, 0x27, 0x69, 0xbe, 0x5a, 0x38, 0x49, 0xf3, 0xee, 0xc2, 0x49, 0x9a, 0xf7, 0x18, 0x4e, 0xd2, 0x7c, 0xc8, 0x70, 0x92, 0xe6, 0xa3, 0x86, 0x93, 0x34, 0x9f, 0x36, 0x9c, 0xa4, 0xf9, 0xcc, 0xe1, 0x24, 0xcd, 0x97, 0x0e, 0x27, 0x69, 0xbe, 0x79, 0x38, 0x49, 0xf3, 0xef, 0xc3, 0x49, 0x9a, 0xef, 0x20, 0x07, 0x21, 0x93, 0x87, 0xc8, 0xe4, 0x39, 0x32, 0xcd, 0x92, 0x4c, 0xf1, 0x25, 0x53, 0xba, 0xc9, 0xd4, 0x74, 0x12, 0xdf, 0x50, 0x5e, 0x2e, 0xe5, 0xe5, 0x55, 0xa6, 0x99, 0x95, 0xe8, 0xcd, 0xb2, 0x42, 0x4b, 0x96, 0x26, 0x97, 0x67, 0x79, 0x89, 0x2c, 0x30, 0x8b, 0xc4, 0xb4, 0x4e, 0xe6, 0x31, 0x67, 0x5e, 0x94, 0xe6, 0x79, 0x6a, 0x9e, 0xdb, 0xe6, 0xf9, 0x6e, 0xde, 0x14, 0xe7, 0x71, 0x72, 0x9e, 0x5f, 0x27, 0xbe, 0xef, 0xbc, 0x1e, 0x4f, 0x3c, 0xed, 0x79, 0x47, 0x9f, 0x78, 0xf2, 0x33, 0xc5, 0x01, 0x4d, 0x2f, 0x41, 0x4f, 0x22, 0xf4, 0x1c, 0x43, 0x4f, 0x4b, 0xf4, 0x6a, 0x45, 0xcf, 0x6e, 0xf4, 0xae, 0x47, 0x91, 0x0b, 0xe9, 0x5d, 0x92, 0x5e, 0x44, 0x29, 0x0a, 0x2d, 0x45, 0x6e, 0xa6, 0xe7, 0x6a, 0x7a, 0x77, 0xa7, 0xd7, 0x7f, 0x7a, 0x37, 0xa8, 0x77, 0x8e, 0x8a, 0xe6, 0x52, 0xaf, 0x3e, 0xf5, 0x9c, 0x54, 0xcf, 0x58, 0xf5, 0x0e, 0x56, 0xef, 0x75, 0xf5, 0x9e, 0x58, 0xd1, 0x30, 0x2b, 0xf2, 0x6a, 0x45, 0x8d, 0xad, 0xc8, 0xca, 0x35, 0x4d, 0x74, 0x45, 0x76, 0x2f, 0x27, 0x69, 0x7e, 0x7c, 0x4d, 0xcd, 0x60, 0xd3, 0x74, 0xd8, 0x2a, 0x62, 0xad, 0x1a, 0x6b, 0x19, 0x59, 0xd3, 0xcb, 0x16, 0x35, 0x6b, 0xf3, 0xd9, 0xb3, 0xa1, 0x3d, 0x46, 0xda, 0x1a, 0x6a, 0x91, 0x6d, 0xed, 0x3d, 0xd8, 0x9a, 0xdc, 0xf6, 0x44, 0x6e, 0xab, 0xbd, 0xad, 0x0d, 0xf7, 0xac, 0x71, 0xd1, 0x40, 0x6e, 0xad, 0xb9, 0xf5, 0xe9, 0x96, 0xaa, 0x6b, 0xb1, 0xdd, 0x13, 0xdc, 0xbd, 0x09, 0xde, 0x4a, 0x79, 0x8f, 0xaf, 0x17, 0xa5, 0xf6, 0xa2, 0x30, 0xdf, 0x34, 0xd9, 0x17, 0xc5, 0xfc, 0xa6, 0xa9, 0x3f, 0x27, 0x69, 0xde, 0x00, 0x56, 0x69, 0x03, 0x9d, 0xa4, 0xf9, 0x25, 0xe8, 0x24, 0xcd, 0xef, 0x41, 0x27, 0x69, 0xfe, 0x10, 0x3a, 0x49, 0xf3, 0xb7, 0xd0, 0x49, 0x9a, 0xff, 0x86, 0x52, 0xd2, 0x8c, 0x72, 0x28, 0x25, 0xcd, 0xe4, 0x88, 0x52, 0xd2, 0x8c, 0x9a, 0x28, 0x25, 0xcd, 0x54, 0x8a, 0x52, 0xd2, 0x4c, 0xbd, 0x28, 0x25, 0xcd, 0x34, 0x8c, 0x52, 0xd2, 0x4c, 0xdb, 0x28, 0x25, 0xcd, 0x74, 0x8e, 0x52, 0xd2, 0x4c, 0xff, 0x28, 0x25, 0xcd, 0x0c, 0x90, 0x52, 0xd2, 0xcc, 0x14, 0x29, 0x25, 0xcd, 0x5c, 0x92, 0x52, 0xd2, 0xcc, 0x3c, 0x59, 0xbd, 0xa6, 0x94, 0x92, 0x66, 0x96, 0x4a, 0x29, 0x69, 0x66, 0xb5, 0x94, 0x92, 0x66, 0xb6, 0x4b, 0x29, 0x69, 0x66, 0xc7, 0x94, 0x92, 0x66, 0x8e, 0x4c, 0x29, 0x69, 0xe6, 0xd4, 0x94, 0x92, 0x66, 0xca, 0x4d, 0x29, 0x69, 0x66, 0xe4, 0x94, 0x92, 0x66, 0x8a, 0x4e, 0x29, 0x69, 0xc6, 0xf5, 0x94, 0x92, 0x66, 0xb4, 0x4f, 0x29, 0x69, 0x46, 0x07, 0x95, 0x92, 0x66, 0x8c, 0x50, 0x29, 0x69, 0xc6, 0x14, 0x95, 0x92, 0x66, 0xac, 0x51, 0x29, 0x69, 0xc6, 0x26, 0x95, 0x92, 0x66, 0x9c, 0x52, 0x29, 0x69, 0xc6, 0x33, 0x95, 0x92, 0x66, 0xb2, 0x53, 0x29, 0x69, 0xc6, 0x47, 0x95, 0x92, 0x66, 0x82, 0x54, 0x29, 0x69, 0x26, 0x54, 0x95, 0x92, 0x66, 0xa2, 0x55, 0x29, 0x69, 0x26, 0x66, 0x95, 0x92, 0x66, 0x92, 0x56, 0x29, 0x69, 0x26, 0x75, 0x75, 0x92, 0xe6, 0xed, 0xab, 0x94, 0x34, 0x23, 0xc3, 0x3a, 0x49, 0xf3, 0x3a, 0xd6, 0x49, 0x9a, 0x07, 0xb2, 0x4e, 0xd2, 0x2c, 0x99, 0x75, 0x92, 0x66, 0x29, 0xad, 0x93, 0x34, 0xcb, 0x6a, 0x9d, 0xa4, 0x59, 0x6e, 0xeb, 0x24, 0xcd, 0x8a, 0x5b, 0x27, 0x69, 0x56, 0xe2, 0x3a, 0x49, 0xb3, 0x2a, 0xd7, 0x49, 0x9a, 0x85, 0xba, 0x4e, 0xd2, 0xac, 0xdb, 0x75, 0x92, 0xe6, 0x00, 0xaf, 0x93, 0x34, 0xfb, 0x7b, 0x9d, 0xa4, 0xd9, 0xe7, 0xeb, 0x24, 0xcd, 0xde, 0x5f, 0x27, 0x69, 0x76, 0x03, 0x3b, 0x49, 0xb3, 0x2b, 0xd8, 0x49, 0x9a, 0x9d, 0xc2, 0x4e, 0xd2, 0x1c, 0x1c, 0x76, 0x92, 0x66, 0x07, 0xb1, 0x93, 0x34, 0xdb, 0x8a, 0x9d, 0xa4, 0xd9, 0x6a, 0xec, 0x24, 0xcd, 0x96, 0x63, 0x27, 0x69, 0x36, 0x23, 0x3b, 0x49, 0xb3, 0x36, 0xd9, 0x49, 0x9a, 0xf5, 0xca, 0x4e, 0xd2, 0x6c, 0x58, 0x76, 0x92, 0x66, 0x23, 0xb3, 0x93, 0x34, 0x7b, 0x9a, 0x9d, 0xa4, 0x59, 0xe3, 0xec, 0x24, 0xcd, 0xf6, 0x67, 0x27, 0x69, 0x36, 0x41, 0x3b, 0x49, 0x33, 0x3f, 0xda, 0x49, 0x9a, 0xd5, 0xd2, 0x4e, 0xd2, 0xcc, 0x9b, 0x76, 0x92, 0x66, 0x01, 0xb5, 0x93, 0x34, 0x8b, 0xab, 0x9d, 0xa4, 0x59, 0x64, 0xed, 0x24, 0xcd, 0xa1, 0x6b, 0x27, 0x69, 0x5e, 0x61, 0x3b, 0x49, 0x73, 0x24, 0xdb, 0x49, 0x9a, 0x9b, 0xda, 0x4e, 0xd2, 0xdc, 0xda, 0x76, 0x92, 0xe6, 0x36, 0xb7, 0x93, 0x34, 0x77, 0xba, 0x9d, 0xa4, 0xb9, 0xeb, 0xed, 0x24, 0xcd, 0xbd, 0x6f, 0x27, 0x69, 0xee, 0x83, 0x3b, 0x49, 0xf3, 0x30, 0xdc, 0x49, 0x9a, 0x47, 0xe2, 0xd5, 0xab, 0x71, 0x27, 0x69, 0x1e, 0x93, 0x3b, 0x49, 0xf3, 0xa4, 0xdc, 0x49, 0x9a, 0xa7, 0xe6, 0x4e, 0xd2, 0x3c, 0x3b, 0x77, 0x92, 0xe6, 0x39, 0xba, 0x93, 0x34, 0x2f, 0xd2, 0x9d, 0xa4, 0x79, 0xa9, 0xee, 0x24, 0xcd, 0x8d, 0x75, 0x27, 0x69, 0xae, 0xb3, 0x3b, 0x49, 0x73, 0xad, 0xdd, 0x49, 0x9a, 0x33, 0xee, 0x4e, 0xd2, 0x1c, 0x7d, 0x77, 0x92, 0xe6, 0x18, 0xbc, 0x93, 0x34, 0xc7, 0xe3, 0x9d, 0xa4, 0x39, 0x21, 0xef, 0x24, 0xcd, 0xc9, 0x79, 0x27, 0x69, 0x4e, 0xd1, 0x3b, 0x49, 0x73, 0xba, 0xde, 0x49, 0x9a, 0xb3, 0xf6, 0x4e, 0xd2, 0x5c, 0xbd, 0x77, 0x92, 0xe6, 0x1c, 0xbe, 0x93, 0x34, 0xe7, 0xf3, 0x9d, 0xa4, 0xb9, 0xa0, 0xef, 0x24, 0xcd, 0xc5, 0x7d, 0x27, 0x69, 0x2e, 0xf1, 0x3b, 0x49, 0x73, 0xb9, 0xdf, 0x49, 0x9a, 0x2b, 0xfe, 0x4e, 0xd2, 0xfc, 0xfc, 0x97, 0x92, 0x66, 0x6e, }
        ), out var _);
        protected override Dictionary<BitList, byte> Table => mytable;
        protected override ushort TableID => 0x8001;
    }

    public class CompressionHuffmanCodingPrepareTable : CompressionHuffmanCoding
    {
        protected virtual Dictionary<BitList, byte> Table => new Dictionary<BitList, byte>();
        protected virtual UInt16 TableID => 0x8000;

        public override byte[] Compress(byte[] buf)
        {
            var ret = MakeTableBinary(TableID, Table, out var table);

            // ADD DATA FIELDS
            ret.Add(BitList.MakeVariableBits((UInt64)buf.Length));  // [DA] Data Bytes
            foreach (var c in buf)
            {
                ret.Add(table[c]);                    // [DB] Huffman code
                if (ret.ByteCount >= LimitSize)
                {
                    return null;
                }
            }
            ret.AddPad();                             // [DC] padding

            return ret.ToByteArray();
        }

        public override byte[] Decompress(byte[] buf)
        {
            var ret = new BitList();
            var bits = new BitList(buf);
            var tableID = BitList.From(bits.Subbit(0, 16)).ToUInt16();   // [TA]
            var nextBitIndex = 16;
            Debug.Assert(tableID == TableID);
            var table = Table;

            //=== READ HUFFMAN CODE ===
            var len = (int)BitList.GetNumberFromVariableBits(bits.Subbit(nextBitIndex), out var nobits);    // [DA] Data Bytes
            nextBitIndex += nobits;
            var outdat = new List<byte>();
            for (var i = 0; i < len; i++)
            {
                var hb = new BitList();
                for (var j = 0; j < 32768; j++)
                {
                    hb.Add(bits[nextBitIndex++]);
                    if (table.TryGetValue(hb, out var value))
                    {
                        outdat.Add(value);  // [DB]
                        break;
                    }
                }
            }
            return outdat.ToArray();
        }
    }
}